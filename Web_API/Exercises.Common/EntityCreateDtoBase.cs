using System;

namespace Exercises.Common
{
    public class EntityCreateDtoBase
    {
        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Specifies whether this entity is published or not.
        /// </summary>
        public bool Published { get; set; }
    }
}