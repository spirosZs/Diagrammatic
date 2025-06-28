namespace Exercises.Common.Exam.Game
{
    public class UpdateGameExerciseDto  
    {
        /// <summary>
        /// Set this flag in the request body if you need to force proceeding to the next exercise.
        /// </summary>
        public bool SkipFlag { get; set; }

        /// <summary>
        /// A positive/negative number expressing time in seconds, to add/remove to the remaining time to complete this exercise.
        /// </summary>
        public int Time { get; set; }
    }
}