using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
// using Exercises.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Exercises.Helpers
{
    public static class Swagger
    {
        public static void Init(this SwaggerGenOptions c)
        {
            //c.DescribeAllEnumsAsStrings();
//            c.UseReferencedDefinitionsForEnums();
            
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "Diagramatic Api Service", Version = "v1"});

            // c.EnableAnnotations();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath, true);

            var xmlFileModels = $"{Assembly.GetExecutingAssembly().GetName().Name}.Common.xml";
            var xmlPathModels = Path.Combine(AppContext.BaseDirectory, xmlFileModels);
            c.IncludeXmlComments(xmlPathModels, true);

            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

//            c.OperationFilter<ConsumesTypeFilter>();
//            c.SchemaFilter<AddExamplesFilter>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Authorization header using the bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        }

        public static void UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Diagramatic Api Service V1"); });
            app.UseReDoc(c =>
            {
                c.SpecUrl("/swagger/v1/swagger.json");
                c.DocumentTitle = "Diagramatic Api Service V1";
            });
        }
    }
}