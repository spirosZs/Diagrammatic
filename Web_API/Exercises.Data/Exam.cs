using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Exercises.Data
{
    public class Exam : ExerciseCollection
    {
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

        [NotMapped] public Guid ExerciseCollectionId { get; set; }

        public ICollection<ExamParticipation> Participations { get; set; } = new List<ExamParticipation>();

        public DateTime? DateStarted { get; set; }

        public DateTime? DateEnded { get; set; }

        public string ParticipationCode { get; set; }

        [ForeignKey(nameof(CurrentExerciseId))]
        public Exercise CurrentExercise { get; set; }

        public Guid? CurrentExerciseId { get; set; }

        public int TimeToNextExercise
        {
            get
            {
                var itemIndex = ExercisesOrdered
                    .FindIndex(x => x.Id == CurrentExerciseId);
                return ExercisesOrdered
                    .Take(itemIndex + 1)
                    .Select(x => x.TimeToComplete)
                    .Sum();
            }
        }

        public bool HasStarted()
        {
            return DateStarted != null;
        }

        public bool HasEnded()
        {
            return DateEnded != null && DateEnded < DateTime.Now;
        }

        public bool IsOngoing()
        {
            return HasStarted() && !HasEnded();
        }

        public static string GenerateCode()
        {
            var r = new Random();
            //TODO: check for duplicates
            return r.Next(1000, 9999).ToString();
        }
    }
}