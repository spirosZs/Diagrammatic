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

        [NotMapped]
        public List<Exercise> ExercisesOrdered => Exercises.OrderBy(x => x.Weight).ToList();

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