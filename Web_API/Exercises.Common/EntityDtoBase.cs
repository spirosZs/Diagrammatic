using System;

namespace Exercises.Common
{
    public class EntityDtoBase
    {
        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The date this entity was created.
        /// </summary>
        public DateTime Created { get; set; }
        
        /// <summary>
        /// A unique identifier for this entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}