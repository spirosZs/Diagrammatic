using System.ComponentModel.DataAnnotations;
using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.Exam.Dto
{
    public class ExamCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// The exercise collection Guid to create an exam from
        /// </summary>
        [Required]
        public Guid ExerciseCollectionId { get; set; }

        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
    }
}
