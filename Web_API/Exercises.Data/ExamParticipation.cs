using System;

namespace Exercises.Data
{
    public class ExamParticipation
    {
       public Guid ExamId { get; set; }
       public Exam Exam { get; set; }
       public Guid UserId { get; set; }
       public User User { get; set; }
    }
}
