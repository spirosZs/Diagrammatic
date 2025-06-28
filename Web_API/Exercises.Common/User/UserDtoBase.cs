using System;

namespace Exercises.Common.User
{
    public class UserDtoBase
    {
        /// <summary>
        /// The students AM.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string AM { get; set; }

        /// <summary>
        /// Students Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Users nickname.
        /// </summary>
        public string UserName { get; set; }
    }
}
