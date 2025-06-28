using System;
using System.Collections.Generic;

namespace Exercises.Common.PropertyMapping.Dictionaries
{
    public class ExerciseDictionary 
        : PropertyMapping<Data.Exercise>
    {
        private readonly Dictionary<string, PropertyMappingValue> _exercisesPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
            };

        public ExerciseDictionary() 
        {
            MergeDictionaries(_exercisesPropertyMapping);
        }
    }
}