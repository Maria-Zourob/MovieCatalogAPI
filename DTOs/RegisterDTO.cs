using System.ComponentModel.DataAnnotations;

namespace MVCAPI.DTOs
{
    public class RegisterDTO
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        [RegularExpression("^(Admin|Reader)$", ErrorMessage = "Role must be either 'Admin' or 'Reader'.")]
        public string Role { get; set; } = "Reader";
    }
}
