using Exercises.Common.Abstractions;
using Exercises.Core;
using Exercises.Core.Abstractions;
using Exercises.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Exercises.Helpers
{
    public static class CrudServices
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IDiagramService, DiagramService>();
            services.AddScoped<IExerciseCollectionService, ExerciseCollectionService>();
            services.AddScoped<IDiagramExerciseService, DiagramExerciseService>();
            services.AddScoped<IPathExerciseService, PathExerciseService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IProblemPluginManager, ProblemPluginManager>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }
    }
}