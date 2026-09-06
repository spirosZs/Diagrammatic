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

        /// <summary>
        /// Index of the current round in <see cref="ExerciseCollection.ExercisesOrdered"/>,
        /// or -1 when no current exercise is set (or it is not part of this exam).
        /// </summary>
        [NotMapped]
        public int CurrentExerciseIndex => ExercisesOrdered.FindIndex(x => x.Id == CurrentExerciseId);

        /// <summary>
        /// True when the current round is the last one, so its expiry ends the exam
        /// rather than advancing to another exercise.
        /// </summary>
        [NotMapped]
        public bool IsOnLastExercise
        {
            get
            {
                var count = ExercisesOrdered.Count;
                return count == 0 || CurrentExerciseIndex >= count - 1;
            }
        }

        public int TimeToNextExercise
        {
            get
            {
                var ordered = ExercisesOrdered;
                var itemIndex = ordered.FindIndex(x => x.Id == CurrentExerciseId);

                // No current exercise on record: the first round is the pending one, so
                // count only its time. Falling through with -1 would Take(0) and return
                // 0, putting the deadline at DateStarted — already in the past for every
                // client and for the worker, which then advances/expires the round the
                // instant the game starts.
                if (itemIndex < 0)
                {
                    itemIndex = 0;
                }

                return ordered
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
            // DateEnded is only ever stamped with "now", never scheduled ahead, so its
            // mere presence means the game is over. The old `< DateTime.Now` left a
            // sub-tick window right after EndGame in which the exam still reported
            // itself as ongoing, handing clients a live deadline for a finished game.
            return DateEnded != null && DateEnded <= DateTime.Now;
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