using Diagrammatic_test.Components.Shared.Dto;
using System.ComponentModel.DataAnnotations;

namespace Diagrammatic_test.Components.Pages.Exercises.Dto
{
    public class ExerciseDto : EntityDtoBase
    {
        public string ProblemType { get; set; }
        public int Weight { get; set; }
        public int TimeToComplete { get; set; }
        public Guid? ExerciseCollectionId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }

    }

    public class ExerciseCreateDto
    {
        /// <summary>
        /// The problem type identifies the type of the exercise.
        /// </summary>
        public string ProblemType { get; set; }

        /// <summary>
        /// The id of the exercise collection that contains this exercise. You can omit that and place this exercise later to an Exercise Collection.
        /// </summary>
        public Guid? ExerciseCollectionId { get; set; }

        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }

        /// <summary>
        /// The weight attribute is used to sort exercises inside an Exercise Collection.
        /// Lower weight value means that this exercise will be placed above other exercises with bigger weight values.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// The time in which a student has to complete the exercise expressed in seconds.
        /// </summary>
        public int TimeToComplete { get; set; }

        public string Name { get; set; }

        public bool published {  get; set; }
    }
}   
