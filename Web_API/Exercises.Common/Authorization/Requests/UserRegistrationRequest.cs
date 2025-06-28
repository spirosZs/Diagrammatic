using System.ComponentModel.DataAnnotations;

namespace Exercises.Common.Authorization.Requests
{
    public class UserRegistrationRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}