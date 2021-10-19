using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Service.ResponseModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IUserService
    {
        Task<CurrentUser> GetCurrentUser(ClaimsPrincipal principal);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<CurrentUser> GetCurrentUser(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if(user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                return new CurrentUser
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Role = roles
                };
            }
            return null;
        }
    }
}
