using System;

namespace Exercises.Data
{
    public class DiagramExercise : Exercise
    {
        public Guid? DiagramId { get; set; }
        public Diagram Diagram { get; set; }
        public string Code { get; set; }
    }
}