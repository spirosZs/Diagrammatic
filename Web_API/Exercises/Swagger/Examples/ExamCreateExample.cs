using System;
using Exercises.Common.Exam;

namespace Exercises.Swagger.Examples
{
    public class ExamCreateExample : IExampleProvider<ExamCreateDto>
    {
        public object GetExample()
        {
            return new ExamCreateDto
            {
                Name = "Create Exam Example",
                ExerciseCollectionId = Guid.NewGuid(),
                Category = "new"
            };
        }
    }
}