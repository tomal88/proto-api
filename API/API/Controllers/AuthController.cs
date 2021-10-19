using API.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.Services;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var response = await _authService.Login(model.Email, model.Password);
            return StatusCode(response.StatusCode, response.Response);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var response = await _authService.SignUp(model.Email, model.Password);
            return StatusCode(response.StatusCode, response.Response);
        }

        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var response = await _authService.ConfirmEmail(userId, token);
            return response.StatusCode switch
            {
                _ => Redirect($"{_config.GetValue<string>("ClientBaseUrl")}/auth/login")
            };
        }

        [HttpPost]
        [Route("resend-confirm-email")]
        public async Task<IActionResult> ResentConfirmEmailLink([FromBody] EmailModel model)
        {
            var response = await _authService.ResendConfirmEmailLink(model.Email);
            return StatusCode(response.StatusCode, response.Response);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailModel emailModel)
        {
            var response = await _authService.SendPasswordResetLink(emailModel.Email);
            return StatusCode(response.StatusCode, response.Response);
        }

        [HttpPost]
        [Route("reset-password/{userId}")]
        public async Task<IActionResult> ResetPassword(string userId, PasswordResetModel model)
        {
            var response = await _authService.ResetPassword(userId, model.Token, model.NewPassword);
            return StatusCode(response.StatusCode, response.Response);
        }
    }
}
