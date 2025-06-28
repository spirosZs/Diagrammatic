using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Exercises.Data.Abstractions;
namespace Exercises.Data
{
    public abstract class Entity : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public DateTime Created { get; set; }
        
        public string Name { get; set; }
        
        public bool Published { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}