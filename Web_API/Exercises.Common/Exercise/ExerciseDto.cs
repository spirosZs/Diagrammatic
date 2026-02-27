using Exercises.Common.Diagram;
using System;

namespace Exercises.Common.Exercise
{
    public class ExerciseDto : EntityDtoBase
    {
        /// <summary>
        /// The problem type identifies the type of the exercise.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The weight attribute is used to sort exercises inside an Exercise Collection.
        /// Lower weight value means that this exercise will be placed above other exercises with bigger weight values.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// The time in which a student has to complete the exercise expressed in seconds.
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// The id of the exercise collection that contains this exercise.
        /// </summary>
        public Guid? ExerciseCollectionId { get; set; }

        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }

        /// <summary>
        /// If exercise is a Diagram returns its Diagram solution.
        /// </summary> 
        public DiagramCreateDto Diagram { get; set; }

        public Guid? DiagramId { get; set; }
    }
}