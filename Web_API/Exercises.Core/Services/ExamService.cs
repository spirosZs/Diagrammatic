using System;
using System;
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

            // Then include related data
            query = query
                .Include(c => c.Participations)
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as DiagramExercise).Diagram))
                .Include(c => c.CurrentExercise);
        }

        protected override void OnBeforeCreate(Exam exam)
        {
            base.OnBeforeCreate(exam);
            var exerciseCollectionId = exam.ExerciseCollectionId;

            var exerciseCollection = _context
                .ExerciseCollections
                .Include(c => c.Exercises)
                .ThenInclude(exercise => ((exercise as DiagramExercise).Diagram))
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

        public async Task<Exam> GetAsync(string participationCode, CancellationToken token = default)
        {
            var query = _dbset.AsQueryable();
            OnGetSingle(ref query);
            return await query
                .FirstOrDefaultAsync(c => c.ParticipationCode == participationCode, cancellationToken: token);
        }

        public async Task<bool> Participate(Exam exam, Guid userId, CancellationToken token = default)
        {
            var participation = new ExamParticipation
            {
                ExamId = exam.Id,
                UserId = userId
            };
            exam.Participations.Add(participation);
            await _context.SaveChangesAsync(token);
            await _hubContext.NotifyAllEvent(GameEventType.ParticipantEntered, new
            {
                examId = exam.Id,
                userId
            });
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
            if (exam.HasEnded())
            {
                throw new Exception($"Game already ended.");
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

        public async Task<Exercise> GoToNextExercise(Guid examId, CancellationToken token = default)
        {
            var exam = await CheckAndGetExam(examId, token);

            var itemIndex = exam.ExercisesOrdered
                .FindIndex(x => x.Id == exam.CurrentExerciseId);

            if (itemIndex + 1 < exam.ExercisesOrdered.Count)
            {
                var nextExercise = exam.ExercisesOrdered[itemIndex + 1];
                exam.CurrentExerciseId = nextExercise.Id;
                exam.CurrentExercise = nextExercise;
            }

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