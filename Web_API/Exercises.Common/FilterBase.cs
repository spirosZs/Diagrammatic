using System;
using Exercises.Data.Abstractions;

namespace Exercises.Common
{
    public abstract class FilterBase
        : PaginationFilter, IEntityFilter
    {
        /// <summary>
        /// The date that this entity was created
        /// </summary>
        public DateTime? Created { get; set; }
        
        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The field that should be used to order the results.
        /// Defaults to "Name"
        /// </summary>
        public string OrderBy { get; set; } = "Name";
    }
}