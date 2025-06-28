using System.Collections.Generic;
using Exercises.Common.User;

namespace Exercises.Common.Exam
{
    public class ExamParticipantsDto    
    {
        /// <summary>
        /// A list of users who participate in this exam.
        /// </summary>
        public ICollection<StudentDto> Students { get; set; } = new List<StudentDto>();

        /// <summary>
        /// Total number of participants.
        /// </summary>
        public int Count => Students.Count;
    }
}
