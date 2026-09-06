using System;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common.Exam;
using Exercises.Common.Exam.Game;

namespace Exercises.Common.Abstractions
{
    public interface IExamService : ICrudService<Data.Exam, ExamFilter, ExamCreateDto>
    {
        Task<Data.Exam> GetAsync(string participationCode, CancellationToken token = default);
        Task<bool> Participate(Data.Exam exam, Guid userId, CancellationToken token = default);
        Task<bool> Participate(Guid examId, Guid userId, CancellationToken token = default);
        Task<Data.Exam> StartGame(Guid examId, CancellationToken token = default);
        Task<Data.Exam> EndGame(Guid examId, CancellationToken token = default);
        Task<Data.Exam> UpdateGame(Guid examId, UpdateGameDto parameters, CancellationToken token = default);
        Task<Data.Exam> UpdateExercise(Guid examId, UpdateGameExerciseDto parameters, CancellationToken token = default);
        Task<Data.Exercise> GetExercise(Guid examId, CancellationToken token = default);
        Task<Data.Exercise> GoToNextExercise(Guid examId, CancellationToken token = default);

        /// <summary>
        /// Settles any round deadline of an ongoing game that has already passed, and
        /// returns the up-to-date exam. Safe to call on every read of the game state.
        /// </summary>
        Task<Data.Exam> SyncProgress(Guid examId, CancellationToken token = default);

        /// <summary>
        /// The id of the teacher who owns this exam, or null if no such exam exists.
        /// </summary>
        /// <remarks>
        /// Deliberately skips the per-role view filter that <c>GetAsync</c> applies, so
        /// that an ownership decision depends only on who actually owns the exam and not
        /// on what the caller's role happens to be allowed to see.
        /// </remarks>
        Task<Guid?> GetOwnerId(Guid examId, CancellationToken token = default);
    }
}