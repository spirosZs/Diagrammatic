using System;
using System.Collections.Generic;

namespace Exercises.Common.PropertyMapping.Dictionaries
{
    public class SubmissionDictionary : PropertyMapping<Data.Submission>
    {
        private readonly Dictionary<string, PropertyMappingValue> _submissionPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Score", new PropertyMappingValue(new List<string>() {"Score"})},
                {"ExamId", new PropertyMappingValue(new List<string>() {"ExamId"})},
                {"ExerciseId", new PropertyMappingValue(new List<string>() {"ExerciseId"})}
            };

        public SubmissionDictionary()
        {
            MergeDictionaries(_submissionPropertyMapping);
        }
    }
}
