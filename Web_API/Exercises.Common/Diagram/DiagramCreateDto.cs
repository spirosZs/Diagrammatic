using System.ComponentModel.DataAnnotations;

namespace Exercises.Common.Diagram
{
    public class DiagramCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// A url string of an image for this diagram.
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// A json definition of this diagram.
        /// </summary>
        [Required]
        public string Definition { get; set; }
    }
}
