using System;

namespace Exercises.Data.Abstractions
{
    public interface IEntity
    {
        Guid Id { get; set; }
        DateTime Created { get; set; }
        string Name { get; set; }
        User User { get; set; }
        Guid UserId { get; set; }
    }
}