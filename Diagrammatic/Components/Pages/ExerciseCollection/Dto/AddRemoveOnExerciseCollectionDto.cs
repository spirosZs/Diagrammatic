using System.ComponentModel.DataAnnotations;

namespace Diagrammatic_test.Components.Pages.ExerciseCollection.Dto
{
    public class AddRemoveOnExerciseCollectionDto
    {
        /// <summary>
        /// The operation to perform.
        /// </summary>
        [Required]
        public ExerciseCollectionOperation Operation { get; set; }

        /// <summary>
        /// The Exercise id to either add or remove to this Exercise Collection.
        /// </summary>
        [Required]
        public Guid ExerciseId { get; set; }
    }
}
