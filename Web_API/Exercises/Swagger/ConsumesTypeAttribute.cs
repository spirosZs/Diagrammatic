using System;
using System.Collections.Generic;

namespace Exercises.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsumesTypeAttribute : Attribute
    {
        public IEnumerable<Type> Types { get; }

        public ConsumesTypeAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}