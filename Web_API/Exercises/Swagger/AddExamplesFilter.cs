using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Exercises.Swagger
{
    public class AddExamplesFilter : ISchemaFilter
    {
        private readonly IEnumerable<Type> _types;

        public AddExamplesFilter()
        {
            var type = typeof(IExample);
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .ToList();
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var exampleType = _types.FirstOrDefault(t =>
            {
                var type = t.GetInterfaces().FirstOrDefault()?.GenericTypeArguments[0];
                return !(type == null) && (type == context.ApiModel.GetType());
            });


            if (exampleType == null) return;

            dynamic instance = Activator.CreateInstance(exampleType);
            schema.Example = JsonSerializer.Serialize(instance.GetExample());
        }
    }
}