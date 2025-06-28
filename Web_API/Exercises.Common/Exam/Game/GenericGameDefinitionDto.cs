using System;

namespace Exercises.Common.Exam.Game
{
    public class GenericGameDefinitionDto
    {
        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier for this entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}