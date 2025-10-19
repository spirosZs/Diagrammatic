using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.Exercises.Dto.DiagramDto
{
    public class DiagramDto : EntityDtoBase
    {
        /// <summary>
        /// A url string of an image for this diagram.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A json definition of this diagram.
        /// </summary>
        public string Definition { get; set; }
    }
}
