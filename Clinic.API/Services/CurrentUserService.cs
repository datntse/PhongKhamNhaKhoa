using Clinic.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace Clinic.API.Services
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
        String GetUserEmail();
        Task<ApplicationUser> GetUser();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public Guid GetUserId()
        {
            var reuslt =  Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst("UserId")?.Value);
            return reuslt;
        }
        public String GetUserEmail()
        {
            return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        public async Task<ApplicationUser> GetUser()
        {
            var userId = GetUserId();
            return await _userManager.FindByIdAsync(userId.ToString());
        }
    }
}
