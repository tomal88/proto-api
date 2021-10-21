using System.ComponentModel.DataAnnotations;

namespace API.RequestModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required"), MinLength(8), MaxLength(30)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required"), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
