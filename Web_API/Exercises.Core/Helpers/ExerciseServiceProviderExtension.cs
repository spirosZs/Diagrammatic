using System;
using System.Collections.Generic;
using Exercises.Common.Abstractions;

namespace Exercises.Core.Helpers
{
    public static class ExerciseServiceProviderExtension
    {
        public static List<string> ErrorMessages = new List<string>();
        
        public static dynamic GetCrudService(this IServiceProvider serviceProvider, Type entityType)
        {
            return serviceProvider.GetCrudService(entityType.Name);
        }

        public static dynamic GetCrudService(this IServiceProvider serviceProvider, string entityType)
        {
            var assembly = typeof(IExerciseService).Assembly;
            Type serviceType = Type.GetType($"Exercises.Common.Abstractions.I{entityType}Service, {assembly}");
            return serviceProvider.GetService(serviceType);
        }
    }
}