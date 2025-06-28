using System;
using Exercises.Data.Types;

namespace Exercises.Common.Problem
{
    public class ProblemDto
    {
        public Guid Id { get; set; }
        public ProblemType Type { get; set; }
        public object Answer { get; set; }
    }
}