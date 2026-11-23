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
                {"Description", new PropertyMappingValue(new List<string>() {"Description"})}
            };

        public ExerciseCollectionDictionary()
        {
            MergeDictionaries(_exercisesPropertyMapping);
        }
    }
}