using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Exercises.Swagger
{
    public class ConsumesTypeFilter : IOperationFilter
    {
        private readonly IEnumerable<Type> _exampleTypes;

        public ConsumesTypeFilter()
        {
            var type = typeof(IExample);
            _exampleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .ToList();
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType == null)
            {
                return;
            }

            var attribute = context
                .MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<ConsumesTypeAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                return;
            }

            var bodyParam = operation.RequestBody.Content.First().Value.Schema;
            if (bodyParam == null) return;

            foreach (var type in attribute.Types)
            {
                var schema = context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);

                var exampleType = _exampleTypes.FirstOrDefault(t =>
                {
                    var exType = t.GetInterfaces().FirstOrDefault()?.GenericTypeArguments[0];
                    return !(exType == null) && (exType == type);
                });

                if (exampleType == null) return;
                dynamic instance = Activator.CreateInstance(exampleType);


                operation.RequestBody.Content.First().Value.Schema = schema;
                operation.RequestBody.Content.First().Value.Example = new OpenApiRawString(
                    JsonSerializer.Serialize(instance.GetExample())
                );
            }
        }
    }
}