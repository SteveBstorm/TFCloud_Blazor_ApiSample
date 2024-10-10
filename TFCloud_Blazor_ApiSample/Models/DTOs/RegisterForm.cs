using System.ComponentModel.DataAnnotations;

namespace TFCloud_Blazor_ApiSample.Models.DTOs
{
    public class RegisterForm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Nickname { get; set; }
    }
}
