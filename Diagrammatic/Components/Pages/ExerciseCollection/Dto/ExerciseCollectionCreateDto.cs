using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.ExerciseCollection.Dto
{
    public class ExerciseCollectionCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string? Description { get; set; }
    }
}