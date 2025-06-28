//using System;
//using System.Linq;
//using Microsoft.Extensions.DependencyInjection;
//using Swashbuckle.AspNetCore.SwaggerGen;
//
//namespace Exercises.Swagger
//{
//    public static class SwaggerGeneratorOptionsExtensions
//    {
//        public static void AddPolymorphism(this SwaggerGenOptions self, Type[] typesToRegister)
//        {
//            if(typesToRegister == null) throw new ArgumentNullException(nameof(typesToRegister));
//            if(typesToRegister.Any() == false) throw new ArgumentException("types cannot be empty", nameof(typesToRegister));
//            
//            var parser = new TypesPasser(typesToRegister);
//            self.DocumentFilter<PolymorphismDocumentFilter>(parser);
//            self.SchemaFilter<PolymorphismSchemaFilter>(parser);
//        }
//    }
//}