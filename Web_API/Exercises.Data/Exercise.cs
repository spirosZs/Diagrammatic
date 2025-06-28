using System;
using System.Collections.Generic;
using Exercises.Data.Types;

namespace Exercises.Data
{
    public class Exercise : Entity
    {
        public ProblemType ProblemType { get; set; }

        public string Category { get; set; }

        public int Weight { get; set; }
        
        public ExerciseCollection ExerciseCollection { get; set; }
        
        public Guid? ExerciseCollectionId { get; set; }

        public int TimeToComplete { get; set; }
        
        public static Dictionary<string, IEnumerable<EntityOperationType>> Permissions()
        {
            return new Dictionary<string, IEnumerable<EntityOperationType>>
            {
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