using Diagrammatic_test.Components.Shared.Dto;

namespace Diagrammatic_test.Components.Pages.Submissions.Dto
{
    public class SubmissionDto : EntityDtoBase
    {
        public Guid ExamId { get; set; }

        public Guid ExerciseId { get; set; }

        public object Data { get; set; }

        public int Score { get; set; }

        public StudentDto User { get; set; }
    }
}
