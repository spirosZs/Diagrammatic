using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.ExerciseCollection.Dto
{
    public class ExerciseCollectionDto : EntityDtoBase
    {
        /// <summary>
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string? Description { get; set; }

        /// <summary>
        /// Total time for the exercises of this collection in seconds.
        /// </summary>
        public int TimeToComplete { get; set; }

        /// <summary>
        /// Total number of exercises.
        /// </summary>
        public int TotalExercises { get; set; }
    }
}
