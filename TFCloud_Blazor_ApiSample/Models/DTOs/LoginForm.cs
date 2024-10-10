using System.ComponentModel.DataAnnotations;

namespace TFCloud_Blazor_ApiSample.Models.DTOs
{
    public class LoginForm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
