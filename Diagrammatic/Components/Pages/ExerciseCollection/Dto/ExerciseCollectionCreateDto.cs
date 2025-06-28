using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.ExerciseCollection.Dto
{
    public class ExerciseCollectionCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
    }
}