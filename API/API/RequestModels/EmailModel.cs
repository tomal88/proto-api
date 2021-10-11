using System.ComponentModel.DataAnnotations;

namespace API.RequestModels
{
    public class EmailModel
    {
        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; }
    }
}
