using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Exercises.Data.Types;

namespace Exercises.Data
{
    public class Submission : Entity
    {
        public int Score { get; set; }
        
        public Guid ExamId { get; set; }
        
        [ForeignKey(nameof(ExamId))]
        public Exam Exam { get; set; }
        
        public Guid ExerciseId { get; set; }
        
        [ForeignKey(nameof(ExerciseId))]
        public Exercise Exercise { get; set; }
        
        public string Data { get; set; }
        
        public static Dictionary<string, IEnumerable<EntityOperationType>> Permissions()
        {
            return new Dictionary<string, IEnumerable<EntityOperationType>>
            {
                {
                    Constants.ROLE_TEACHER, new List<EntityOperationType>
                    {
                        EntityOperationType.ViewAny,
                    }
                },
                {
                    Constants.ROLE_STUDENT, new List<EntityOperationType>()
                    {
                        EntityOperationType.Create,
//                        EntityOperationType.ViewOwn
                    }
                }
            };
        }
    }
}