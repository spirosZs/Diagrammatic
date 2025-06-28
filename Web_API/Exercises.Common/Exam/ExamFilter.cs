using System;

namespace Exercises.Common.Exam
{
    /// <summary>
    /// An object that holds query parameters to filter a collection of exams.
    /// </summary>
    public class ExamFilter : FilterBase
    {
        /// <summary>
        /// A string representing a category label for this entity.
        /// </summary> 
        public string Category { get; set; }
    }
}