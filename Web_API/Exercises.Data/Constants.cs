using System.Diagnostics.CodeAnalysis;

namespace Exercises.Data
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Constants
    {
        public const int DEFAULT_TIME_TO_COMPLETE_EXERCISE = 300;
        public const int MAX_SCORE = 100;
        public const int REDUNDANT_ANSWER_NEGATIVE_IMPACT = 5;

        public const string ROLE_ANONYMOUS = "Anonymous";
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_TEACHER = "Teacher";
        public const string ROLE_STUDENT = "Student";

        /// <summary>
        /// Not a user role: the identity given to the timer worker when it presents the
        /// shared service key. It may drive game progression for any exam, which is why
        /// it is never granted to anyone who signs in.
        /// </summary>
        public const string ROLE_SERVICE = "Service";

        /// <summary>
        /// Authorization policy guarding the game hub. Declared here so the hub (in
        /// Exercises.Core) and the policy registration (in the web project) cannot drift.
        /// </summary>
        public const string POLICY_GAME_HUB = "GameHubAccess";
    }
}