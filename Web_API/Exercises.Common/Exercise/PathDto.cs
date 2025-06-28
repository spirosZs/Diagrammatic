namespace Exercises.Common.Exercise
{
    public class PathDto
    {
        /// <summary>
        /// The path definition.
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Defines how important is this path in relation with the other paths for this exercise.
        /// This value affects the final score for this exercise. 
        /// </summary>
        public double Credits { get; set; }
    }
}