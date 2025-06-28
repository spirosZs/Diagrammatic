using System.ComponentModel.DataAnnotations;

namespace Exercises.Data
{
    public class Diagram : Entity
    {
        public string Url { get; set; }

        [Required]
        public string Definition { get; set; }
    }
}
