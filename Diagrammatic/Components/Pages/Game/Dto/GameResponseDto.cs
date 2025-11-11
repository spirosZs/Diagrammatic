namespace Diagrammatic_test.Components.Pages.Game.Dto
{
    class GameResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProblemType { get; set; } = string.Empty;
        public Guid ExerciseCollectionId { get; set; }
        public int TimeToComplete { get; set; }
    }
}
