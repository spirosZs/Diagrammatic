using System;
using System.Collections.Generic;

namespace Exercises.Common.PropertyMapping.Dictionaries
{
    public class ExamDictionary : PropertyMapping<Data.Exam>
    {
        private readonly Dictionary<string, PropertyMappingValue> _exercisesPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Category", new PropertyMappingValue(new List<string>() {"Category"})}
            };

        public ExamDictionary()
        {
            MergeDictionaries(_exercisesPropertyMapping);
        }
    }
}