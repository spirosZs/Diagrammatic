using System;

namespace Exercises.Common.Exam.Game
{
    public static class Extensions
    {
        public static GameInfoDto MapToGameInfoDto(this Data.Exam exam)
        {
            var info = new GameInfoDto
            {
                Id = exam.Id,
                Name = exam.Name,
                Category = exam.Category,
                Created = exam.Created,
                HasStarted = exam.HasStarted(),
                HasEnded = exam.HasEnded(),
                DateStarted = exam.DateStarted,
                DateEnded = exam.DateEnded,
                TotalExercises = exam.TotalExercises,
                Participants = exam.Participations.Count,
                CurrentExerciseId = exam.CurrentExerciseId,
                CurrentExerciseIndex = exam.CurrentExerciseIndex,
                IsOnLastExercise = exam.HasStarted() && exam.IsOnLastExercise,
            };
            if (exam.IsOngoing())
            {
                var startedDateTime = (DateTime)exam.DateStarted;

                info.DateTimeToEnd = startedDateTime.AddSeconds(exam.TimeToComplete);
                info.DateTimeToNextExercise = startedDateTime.AddSeconds(exam.TimeToNextExercise);
            }
            return info;
        }
    }
}