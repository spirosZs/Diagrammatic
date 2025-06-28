using System.Collections.Generic;

namespace Exercises.Common
{
    public class AuthenticationResult
    {
        public const string UserWithMailAlreadyExists = "User with this email address already exists";
        public const string UserDoesNotExist = "User does not exist";
        public const string WrongPassword = "User/password combination is wrong";
        public const string InvalidToken = "Invalid Token";
        public const string TokenNotExpiredYet = "This token hasn't expired yet";

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}