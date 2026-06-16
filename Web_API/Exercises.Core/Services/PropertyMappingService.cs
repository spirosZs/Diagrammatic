using System;
using System.Collections.Generic;
using System.Linq;
using Exercises.Common.Abstractions;
using Exercises.Common.PropertyMapping;
using Exercises.Common.PropertyMapping.Dictionaries;
using Exercises.Core.Abstractions;

namespace Exercises.Core.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new ExerciseDictionary());
            propertyMappings.Add(new ExerciseCollectionDictionary());
            propertyMappings.Add(new ExamDictionary());
            propertyMappings.Add(new SubmissionDictionary());
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TResource>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TResource>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception(
                $"Cannot find exact property mapping instance for {typeof(TResource)}");
        }

        public bool ValidMappingExistsFor<TDictionary>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TDictionary>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                // remove everything after the first " " - if the fields are coming from an orderBy string, this part must be ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}