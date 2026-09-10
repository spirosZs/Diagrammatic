using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Diagrammatic2.Components.Shared.Requests
{
    public class UserRegistrationRequest
    {
        /// <summary>
        /// Shown next to the password box and reused as the validation message, so the
        /// rule the form advertises and the rule it enforces cannot drift apart.
        /// </summary>
        public const string PasswordRule =
            "Password must be at least 8 characters and contain an uppercase letter, a lowercase letter, a number and a special character.";

        // Mirrors PasswordRule. ASP.NET Identity on the API enforces the same character
        // classes (its defaults) but only a 6 character minimum, so this is the stricter
        // of the two and a password accepted here is always accepted there.
        private const string PasswordPattern =
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$";

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(PasswordPattern, ErrorMessage = PasswordRule)]
        public string password { get; set; }

        /// <summary>
        /// Never sent to the server — it exists so a typo is caught on the form instead of
        /// locking the user out of the account they just created.
        /// </summary>
        [JsonIgnore]
        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare(nameof(password), ErrorMessage = "The passwords do not match.")]
        public string confirmPassword { get; set; }

        [Required]
        public string role { get; set; }
    }
}
