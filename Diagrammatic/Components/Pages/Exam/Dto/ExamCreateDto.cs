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
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string? Description { get; set; }

        public string ParticipationCode {  get; set; }
    }
}
