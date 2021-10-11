using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.ResponseModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IAuthService
    {
        Task<Result<object>> Login(string email, string password);
        Task<Result<object>> SignUp(string email, string password);
        Task<Result<object>> ConfirmEmail(string userId, string token);
        Task<Result<object>> SendPasswordResetLink(string email);
        Task<Result<object>> ResetPassword(string userId, string token, string newPassword);
        Task<Result<object>> ResendConfirmEmailLink(string email);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        
        public async Task<Result<object>> SignUp(string email, string password)
        {
            var isUserExist = await _userManager.FindByEmailAsync(email);
            if (isUserExist != null)
                return new Result<object> { StatusCode = 400, Response = new { Message = "Account already exists with this email!" } };

            ApplicationUser user = new()
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return new Result<object> { StatusCode = 500, Response = new { Message = "Sign up failed, please try again" } };

            await _userManager.AddToRoleAsync(user, UserRole.GeneralUser.ToString());

            var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(user)));
            await _emailService.SendEmailConfirmationMail(user.Id, email, token);

            return new Result<object> { StatusCode = 200, Response = new { Message = "Successfully signed up, Please check your email to confirm" } };
        }

        public async Task<Result<object>> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var decodedToken = WebEncoders.Base64UrlDecode(token);
                var result = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(decodedToken));
                if (result.Succeeded)
                {
                    return new Result<object> { StatusCode = 200, Response = new { Message = "Email confirmed" } };
                }
            }
            return new Result<object> { StatusCode = 200, Response = new { Message = "Invalid token" } };
        }

        public async Task<Result<object>> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                if (!user.EmailConfirmed)
                {
                    return new Result<object> { StatusCode = 400, Response = new { Message = "Please confirm your email first" } };
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key:SecretKey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["ApiBaseUrl"],
                    expires: DateTime.Now.AddDays(7),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new Result<object> { StatusCode = 200, Response = new { Token = new JwtSecurityTokenHandler().WriteToken(token) } };
            }
            return new Result<object> { StatusCode = 404, Response = new { Message = "Username or Password doesn't match" } };
        }

        public async Task<Result<object>> SendPasswordResetLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                await _emailService.SendForgotPasswordMail(user.Id, email, encodedToken);
                return new Result<object> { StatusCode = 404, Response = new { Message = "Password reset link is sent via email" } };
            }
            return new Result<object> { StatusCode = 404, Response = new { Message = "No user found for this email" } };
        }

        public async Task<Result<object>> ResetPassword(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var decodedToken = WebEncoders.Base64UrlDecode(token);
                var response = await _userManager.ResetPasswordAsync(user, Encoding.UTF8.GetString(decodedToken), newPassword);
                if (response.Succeeded)
                {
                    return new Result<object> { StatusCode = 200, Response = new { Message = "Successfully reset password" } };
                }
            }
            return new Result<object> { StatusCode = 400, Response = new { Message = "Invalid token" } };
        }

        public async Task<Result<object>> ResendConfirmEmailLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _userManager.GenerateEmailConfirmationTokenAsync(user)));
                await _emailService.SendEmailConfirmationMail(user.Id, email, token);
                return new Result<object> { StatusCode = 400, Response = new { Message = "Successfully sent, please check your email" } };
            }
            return new Result<object> { StatusCode = 404, Response = new { Message = "Email not found" } };
        }
    }
}
