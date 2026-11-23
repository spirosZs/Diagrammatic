namespace Exercises.Common.Exam
{
    public class ExamUpdateDto : EntityUpdateDtoBase
    {
        /// <summary>
        /// Free text describing this entity. Optional.
        /// </summary> 
        public string Description { get; set; }

    }
}