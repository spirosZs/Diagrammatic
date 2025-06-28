using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Exercises.Data
{
    public class User : IdentityUser<Guid>
    {
        // ReSharper disable once InconsistentNaming
        public string AM { get; set; }

        public ICollection<ExamParticipation> Participations { get; set; } = new List<ExamParticipation>();
    }
}