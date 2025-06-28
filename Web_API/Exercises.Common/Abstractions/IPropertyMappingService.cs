using System.Collections.Generic;
using Exercises.Common.PropertyMapping;

namespace Exercises.Common.Abstractions
{
    public interface IPropertyMappingService
    {
        bool ValidMappingExistsFor<TResource>(string fields);

        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TResource>();
    }
}
