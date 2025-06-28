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
    }
}