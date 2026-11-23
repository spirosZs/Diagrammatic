namespace Exercises.Common.ExerciseCollection
{
    public class ExerciseCollectionCreateDto : EntityCreateDtoBase
    {
        /// <summary>
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string Description { get; set; }
        
    }
}