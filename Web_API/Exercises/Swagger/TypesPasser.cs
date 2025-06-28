using System;

namespace Exercises.Swagger
{
    public class TypesPasser
    {
        public TypesPasser(Type[] types)
        {
            Types = types;
        }

        public Type[] Types { get; }
    }
}