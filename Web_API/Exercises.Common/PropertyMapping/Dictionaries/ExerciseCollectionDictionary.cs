using System;
using System.Collections.Generic;

namespace Exercises.Common.PropertyMapping.Dictionaries
{
    public class ExerciseCollectionDictionary
        : PropertyMapping<Data.ExerciseCollection>
    {
        private readonly Dictionary<string, PropertyMappingValue> _exercisesPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Category", new PropertyMappingValue(new List<string>() {"Category"})}
            };

        public ExerciseCollectionDictionary()
        {
            MergeDictionaries(_exercisesPropertyMapping);
        }
    }
}