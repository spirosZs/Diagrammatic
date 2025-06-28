namespace Exercises.Common.Exam
{
    public class ExamUpdateDto : EntityUpdateDtoBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }

    }
}