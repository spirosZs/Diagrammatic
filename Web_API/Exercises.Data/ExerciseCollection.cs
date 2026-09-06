using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Exercises.Data.Types;

namespace Exercises.Data
{
    public class ExerciseCollection : Entity
    {
        [Required] public string Category { get; set; }

        public int TimeToComplete => Exercises.Select(x => x.TimeToComplete).Sum();

        public int TotalExercises => Exercises.Count;

        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();

        // The round order of a game is derived from this list by *position*
        // (Exam.CurrentExerciseId is resolved to an index), so the sort has to be
        // total and identical on every request. Weight alone is not: it defaults to
        // 0, so a teacher who leaves it alone gets ties, and OrderBy then just
        // preserves whatever order EF materialised Exercises in. That order is not
        // guaranteed — the Exam query LEFT JOINs Participations/Diagrams, so the
        // plan (and with it the row order) can change as students join, which
        // silently renumbered the rounds mid-game. Created, then Id, break the tie
        // deterministically.
        [NotMapped]
        public List<Exercise> ExercisesOrdered => Exercises
            .OrderBy(x => x.Weight)
            .ThenBy(x => x.Created)
            .ThenBy(x => x.Id)
            .ToList();

        public static Dictionary<string, IEnumerable<EntityOperationType>> Permissions()
        {
            return new Dictionary<string, IEnumerable<EntityOperationType>>
            {
                {
                    Constants.ROLE_ANONYMOUS, new List<EntityOperationType>
                    {
                        EntityOperationType.ViewAny
                    } 
                },
                {
                    Constants.ROLE_TEACHER, new List<EntityOperationType>
                    {
                        EntityOperationType.ViewOwn,
                        EntityOperationType.Create,
                        EntityOperationType.UpdateOwn,
                        EntityOperationType.DeleteOwn
                    }
                },
                {
                    Constants.ROLE_STUDENT, new List<EntityOperationType>()
                    {
                        EntityOperationType.ViewAny
                    }
                }
            };
        }
    }
}