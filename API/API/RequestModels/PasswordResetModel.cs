using System.ComponentModel.DataAnnotations;

namespace API.RequestModels
{
    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Invalid token")]
        public string Token { get; set; }
        [Required(ErrorMessage = "Password is required"), MinLength(8), MaxLength(30)]
        public string NewPassword { get; set; }
    }
}
