using System;
using System.Collections.Generic;
using System.Linq;
using Exercises.Core.Abstractions;

namespace Exercises.Common.PropertyMapping
{
    public class PropertyMapping<TResource> : IPropertyMapping
    {
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; set; } =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>() {"Id"})},
                {"Name", new PropertyMappingValue(new List<string>() {"Name"})},
                {"Created", new PropertyMappingValue(new List<string>() {"Created"})}
            };


        protected void MergeDictionaries(Dictionary<string, PropertyMappingValue> dictionaryFrom)
        {
            dictionaryFrom
                .ToList()
                .ForEach(x => MappingDictionary[x.Key] = x.Value);
        }
    }
}