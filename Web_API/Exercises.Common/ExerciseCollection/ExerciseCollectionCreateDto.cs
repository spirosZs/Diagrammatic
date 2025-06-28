namespace Exercises.Common.ExerciseCollection
{
    public class ExerciseCollectionCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
        
    }
}