using System.ComponentModel.DataAnnotations;

namespace Diagrammatic2.Components.Shared.Requests
{
    public class UserRegistrationRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        public string role { get; set; }
    }
}
