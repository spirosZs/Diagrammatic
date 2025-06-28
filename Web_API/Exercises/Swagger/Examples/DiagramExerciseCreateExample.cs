using System;
using Exercises.Common.Diagram;
using Exercises.Common.Exercise;
using Exercises.Data.Types;


namespace Exercises.Swagger.Examples
{
    public class DiagramExerciseCreateExample : IExampleProvider<DiagramExerciseCreateDto>
    {
        public object GetExample()
        {
            return new DiagramExerciseCreateDto
            {
                ExerciseCollectionId = Guid.NewGuid(),
                ProblemType = ProblemType.Diagram,
                Name = "New diagram exercise",
                Category = "Diagrams",
                Weight = 5,
                Code = "hello world",
                Diagram = new DiagramCreateDto()
                {
                    Url = "image url",
                    Name = "First Diagram",
                    Definition = "json definition goes here"
                }
            };
        }
    }
}