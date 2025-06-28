using System;

namespace Exercises.Data.Abstractions
{
    public interface IEntityFilter
    {
        DateTime? Created { get; set; }
        
        string Name { get; set; }
        
    }
}