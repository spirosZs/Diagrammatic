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
    }
}