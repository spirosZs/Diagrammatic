using Diagrammatic_test.Components.Pages.Exercises.Dto.Diagram;
using Diagrammatic_test.Components.Shared.Dto;
namespace Diagrammatic_test.Components.Pages.Exercises.Dto
{
    public class ExerciseDto : EntityDtoBase
    {
        public string ProblemType { get; set; } = string.Empty;
        public int Weight { get; set; }
        public int TimeToComplete { get; set; }
        public Guid? ExerciseCollectionId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        // 🔹 Embedded diagram (resolved by backend)
        public DiagramDto Diagram { get; set; } = default!;
    }




    public class ExerciseCreateDto
    {
        public string ProblemType { get; set; }
        public Guid? ExerciseCollectionId { get; set; }
        public string Category { get; set; }
        public int Weight { get; set; }
        public int TimeToComplete { get; set; }
        public string Name { get; set; }
        public bool Published { get; set; }

        // 🔥 THIS is all you need
        public Guid? DiagramId { get; set; }
    }

}
