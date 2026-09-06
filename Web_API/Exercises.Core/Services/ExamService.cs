using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common.Abstractions;
using Exercises.Common.Exam;
using Exercises.Common.Exam.Game;
using Exercises.Core.Helpers;
using Exercises.Core.Hubs;
using Exercises.Data;
using Exercises.Data.DbContext;
using Exercises.Data.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Core.Services
{
    public class ExamService :
        ResourceBase<Exam, ExamFilter, ExamCreateDto>, IExamService
    {
        private readonly IHubContext<GameHub> _hubContext;

        public ExamService(ExercisesContext context, IPropertyMappingService propertyMappingService,
            IHttpContextAccessor httpContextAccessor, IHubContext<GameHub> hubContext)
            : base(context, propertyMappingService, httpContextAccessor)
        {
            _hubContext = hubContext;
        }

        protected override void OnGetSingle(ref IQueryable<Exam> query)
        {
            // First apply base authorization and filters
            base.OnGetSingle(ref query);

            // Then include related data.
            //
            // DiagramExercise and PathExercise each declare their own Diagram navigation
            // (TPH gives them separate FK columns, DiagramId and PathExercise_DiagramId), so
            // both have to be included explicitly. Without the PathExercise one a path
            // exercise reaches the client with Diagram == null and the student sees neither
            // the exercise image nor its definition.
            //
            // CurrentExercise needs the same two includes of its own, even though it always
            // points at a row that is already in Exercises. GetUntracked runs this query
            // with AsNoTracking, which does no identity resolution: the row materializes
            // twice, as separate instances, so the diagram loaded onto the one in Exercises
            // never reaches the one behind CurrentExercise — and CurrentExercise is what
            // GET /api/game/{id}/exercise returns. That path is taken while a round is
            // still running (NeedsSettling == false), which is exactly when a student is
            // looking at the exercise, so the image was missing during every live round and
            // present in every after-the-fact check.
            query = query
                .Include(c => c.Participations)
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as DiagramExercise).Diagram))
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as PathExercise).Diagram))
                .Include(c => c.CurrentExercise)
                .ThenInclude(exercise => ((exercise as DiagramExercise).Diagram))
                .Include(c => c.CurrentExercise)
                .ThenInclude(exercise => ((exercise as PathExercise).Diagram));
        }

        protected override void OnBeforeCreate(Exam exam)
        {
            base.OnBeforeCreate(exam);
            var exerciseCollectionId = exam.ExerciseCollectionId;

            var exerciseCollection = _context
                .ExerciseCollections
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as DiagramExercise).Diagram))
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as PathExercise).Diagram))
                .FirstOrDefault(c => c.Id == exerciseCollectionId);

            if (exerciseCollection == null)
            {
                throw new Exception($"Collection with id {exerciseCollectionId} was not found.");
            }

            _context.DetachEntity(exerciseCollection);


            foreach (var exercise in exerciseCollection.Exercises)
            {
                _context.DetachEntity(exercise);
                var diagram = (Diagram) exercise.GetType().GetProperty("Diagram")?.GetValue(exercise, null);
                if (diagram != null)
                {
                    _context.DetachEntity(diagram);
                }
                exam.Exercises.Add(exercise);
                exercise.ExerciseCollectionId = exam.Id; 
            }
            
            // exam.CurrentExerciseId = Guid.Empty;
            // exam.Id = new Guid();
            // exam.CurrentExercise = exam.ExercisesOrdered.First();
        }

        public async Task<Guid?> GetOwnerId(Guid examId, CancellationToken token = default)
        {
            var owners = await _dbset
                .AsNoTracking()
                .Where(c => c.Id == examId)
                .Select(c => (Guid?) c.UserId)
                .FirstOrDefaultAsync(token);

            return owners;
        }

        public async Task<Exam> GetAsync(string participationCode, CancellationToken token = default)
        {
            var query = _dbset.AsQueryable();
            OnGetSingle(ref query);
            return await query
                .FirstOrDefaultAsync(c => c.ParticipationCode == participationCode, cancellationToken: token);
        }

        public async Task<bool> Participate(Exam exam, Guid userId, CancellationToken token = default)
        {
            // Avoid duplicate participations. The exam is loaded together with its
            // Participations, so re-joining (page refresh / re-entering the code) would
            // otherwise try to track a second ExamParticipation with the same
            // {ExamId, UserId} key and throw an InvalidOperationException.
            var alreadyJoined = exam.Participations.Any(p => p.UserId == userId);
            if (!alreadyJoined)
            {
                exam.Participations.Add(new ExamParticipation
                {
                    ExamId = exam.Id,
                    UserId = userId
                });
                await _context.SaveChangesAsync(token);
                await _hubContext.NotifyAllEvent(GameEventType.ParticipantEntered, new
                {
                    examId = exam.Id,
                    userId
                });
            }

            return true;
        }

        public async Task<bool> Participate(Guid examId, Guid userId, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);
            return await Participate(exam, userId, token);
        }

        public async Task<Exam> StartGame(Guid examId, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);
            if (exam.HasStarted())
            {
                throw new Exception($"Game already started");
            }

            // Ensure there are exercises available before starting
            if (exam.ExercisesOrdered == null || !exam.ExercisesOrdered.Any())
            {
                throw new Exception("Cannot start game: no exercises are configured for this exam.");
            }

            var currentDate = DateTime.Now;
            exam.DateStarted = currentDate;

            // Set the current exercise to the first available exercise
            var firstExercise = exam.ExercisesOrdered.FirstOrDefault();
            if (firstExercise == null)
            {
                throw new Exception("Cannot start game: failed to determine the first exercise.");
            }

            exam.CurrentExercise = firstExercise;

            await _context.SaveChangesAsync(token);
            await _hubContext.NotifyAllEvent(GameEventType.Started, new { examId });

            return exam;
        }

        public async Task<Exam> EndGame(Guid examId, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);

            // Idempotent: the final round expiring ends the game from GoToNextExercise,
            // and the worker's separate "end of game" timeout invokes this a moment
            // later for the same exam. Throwing there faulted the hub invocation and
            // re-broadcast nothing useful, so just report the already-ended exam.
            if (exam.DateEnded != null)
            {
                return exam;
            }

            exam.DateEnded = DateTime.Now;
            await _context.SaveChangesAsync(token);
            await _hubContext.NotifyAllEvent(GameEventType.Ended, new {examId});
            return exam;
        }

        public async Task<Exam> UpdateGame(Guid examId, UpdateGameDto parameters, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);
            if (parameters.ResetParticipationCodeFlag)
            {
                exam.ParticipationCode = Exam.GenerateCode();
            }

            // if (parameters.RestartGameFlag)
            // {
            //     //TODO: handle restart
            // }
            await _context.SaveChangesAsync(token);
            await _hubContext.NotifyAllEvent(GameEventType.Updated, new {examId});
            return exam;
        }

        public async Task<Exam> UpdateExercise(Guid examId, UpdateGameExerciseDto parameters,
            CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);
            if (!exam.IsOngoing())
            {
                throw new Exception("Game is not active. Start the game first and update when necessary.");
            }

            var exercise = exam.CurrentExercise;
            if (parameters.SkipFlag)
            {
                await GoToNextExercise(examId, token);
            }
            else if (parameters.Time != 0)
            {
                exercise.TimeToComplete += parameters.Time;
                await _context.SaveChangesAsync(token);
                await _hubContext.NotifyAllEvent(GameEventType.ExerciseTimeChanged, new {examId});
            }

            return exam;
        }

        public async Task<Exercise> GetExercise(Guid examId, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);
            return exam.CurrentExercise;
        }

        /// <summary>
        /// Brings an ongoing game in line with the wall clock, advancing past every
        /// round whose deadline has already passed and ending the exam once the final
        /// round has expired.
        /// </summary>
        /// <remarks>
        /// Round progression is otherwise driven entirely by the external worker's
        /// in-memory timers, which are only ever armed in response to a live SignalR
        /// event. So a worker restart — or a dropped hub connection between the start of
        /// a round and its deadline — silently orphaned the game: nothing advanced it
        /// again and every student sat on the "waiting for the next exercise" screen for
        /// the rest of the exam. Settling elapsed deadlines on each read of the game
        /// state makes progression correct whether or not the worker is healthy.
        /// </remarks>
        public async Task<Exam> SyncProgress(Guid examId, CancellationToken token = default)
        {
            // Deliberately untracked. This probe runs before the gate, so if it were
            // tracked it would put the exam in the context's identity map, and the
            // re-read inside the gate would then hand back that same stale instance
            // instead of the state the previous caller just committed — every waiter
            // would advance and broadcast again, which is the exact bug the gate exists
            // to prevent.
            var exam = await GetUntracked(examId, token);
            if (exam == null)
            {
                throw new Exception($"Exam with id {examId} doesn't exist.");
            }

            // Fast path, and the one almost every request takes: the current round still
            // has time on it, so there is nothing to settle and no reason to queue behind
            // anyone. Serialising these too cost about a third of the read throughput at
            // 60 concurrent clients, for no benefit.
            if (!NeedsSettling(exam))
            {
                return exam;
            }

            // Settling a deadline is a read-modify-write. With a class polling together,
            // the requests that cross a round deadline all used to load the same
            // pre-advance exam, all advance it, and all broadcast: measured 14
            // ExerciseCompleted events for a single transition, each of which makes every
            // connected client re-fetch. Serialising per exam means the first caller
            // advances and announces it, and the rest then re-read the already-settled
            // state and stay quiet.
            var gate = ProgressGates.GetOrAdd(examId, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(token);
            try
            {
                return await SyncProgressCore(examId, token);
            }
            finally
            {
                gate.Release();
            }
        }

        /// <summary>
        /// Loads the exam exactly as <see cref="GetAsync(Guid,CancellationToken)"/> does but
        /// without tracking it, so reading it cannot affect what a later tracked query in
        /// the same request sees.
        /// </summary>
        private async Task<Exam> GetUntracked(Guid examId, CancellationToken token)
        {
            var query = _dbset.AsNoTracking();
            OnGetSingle(ref query);
            return await query.FirstOrDefaultAsync(c => c.Id == examId, cancellationToken: token);
        }

        /// <summary>
        /// True when an ongoing game's current round has run out of time, i.e. when
        /// <see cref="SyncProgressCore"/> would actually change something.
        /// </summary>
        private static bool NeedsSettling(Exam exam)
        {
            return exam.HasStarted()
                   && exam.DateEnded == null
                   && exam.DateStarted.Value.AddSeconds(exam.TimeToNextExercise) <= DateTime.Now;
        }

        // Keyed by exam, so unrelated games never wait on each other. This guards a single
        // process; a scaled-out deployment would need the equivalent guard in the database
        // (a conditional UPDATE on CurrentExerciseId) instead.
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> ProgressGates =
            new ConcurrentDictionary<Guid, SemaphoreSlim>();

        private async Task<Exam> SyncProgressCore(Guid examId, CancellationToken token)
        {
            // Loaded inside the gate: a caller that queued behind the winner has to see
            // the state the winner committed, not the snapshot it queued with.
            var exam = await CheckAndGetExam(examId, token);

            if (!exam.HasStarted() || exam.DateEnded != null)
            {
                return exam;
            }

            var startedAt = exam.DateStarted.Value;
            var ordered = exam.ExercisesOrdered;
            var advanced = false;
            var ended = false;

            // Each pass either settles one round or stops, so the number of rounds
            // bounds the loop. Catching up several at once matters when the process (or
            // the worker) was down across more than one deadline.
            for (var guard = 0; guard <= ordered.Count; guard++)
            {
                if (startedAt.AddSeconds(exam.TimeToNextExercise) > DateTime.Now)
                {
                    break; // the current round still has time on the clock
                }

                var nextIndex = ordered.FindIndex(x => x.Id == exam.CurrentExerciseId) + 1;
                if (nextIndex >= ordered.Count)
                {
                    exam.DateEnded = DateTime.Now;
                    ended = true;
                    break;
                }

                exam.CurrentExerciseId = ordered[nextIndex].Id;
                exam.CurrentExercise = ordered[nextIndex];
                advanced = true;
            }

            if (!advanced && !ended)
            {
                return exam;
            }

            await _context.SaveChangesAsync(token);
            await _hubContext.NotifyAllEvent(
                ended ? GameEventType.Ended : GameEventType.ExerciseCompleted,
                new {examId});

            return exam;
        }

        public async Task<Exercise> GoToNextExercise(Guid examId, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);

            // Nothing to advance in a game that hasn't started or is already over.
            if (!exam.HasStarted() || exam.DateEnded != null)
            {
                return exam.CurrentExercise;
            }

            var ordered = exam.ExercisesOrdered;

            // -1 (current exercise unknown) becomes 0, i.e. recover onto the first round
            // instead of silently doing nothing.
            var nextIndex = ordered.FindIndex(x => x.Id == exam.CurrentExerciseId) + 1;

            if (nextIndex >= ordered.Count)
            {
                // Already on the final round, so its expiry is the end of the exam.
                //
                // This used to fall through and broadcast ExerciseCompleted without
                // moving the round on, which is what produced the runaway loop: the
                // worker re-arms its timers from every ExerciseCompleted, recomputes a
                // DateTimeToNextExercise that is already in the past, and setTimeout
                // with a negative delay fires immediately — calling straight back into
                // here. Every connected student got that event storm too, and stayed
                // pinned on the "waiting for the next exercise" screen because the round
                // never actually changed.
                exam.DateEnded = DateTime.Now;
                await _context.SaveChangesAsync(token);
                await _hubContext.NotifyAllEvent(GameEventType.Ended, new {examId});

                return exam.CurrentExercise;
            }

            var nextExercise = ordered[nextIndex];
            exam.CurrentExerciseId = nextExercise.Id;
            exam.CurrentExercise = nextExercise;

            await _context.SaveChangesAsync(token);
            await _hubContext.NotifyAllEvent(GameEventType.ExerciseCompleted, new {examId});

            return exam.CurrentExercise;
        }


        public async Task<Exam> CheckAndGetExam(Guid examId, CancellationToken token)
        {
            var exam = await GetAsync(examId, token);
            if (exam == null)
            {
                throw new Exception($"Exam with id {examId} doesn't exist.");
            }

            return exam;
        }
    }
}