using Exercises.Common.ExerciseCollection;

namespace Exercises.Swagger.Examples
{
    public class ExerciseCollectionCreateExample : IExampleProvider<ExerciseCollectionCreateDto>
    {
        public object GetExample()
        {
            return new ExerciseCollectionCreateDto
            {
              Name  = "Dummy exercise collection",
              Category = "new"
            };
        }
    }
}