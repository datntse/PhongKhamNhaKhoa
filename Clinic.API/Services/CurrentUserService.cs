using AutoMapper;
using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstruture.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace Clinic.API.Services
{
    public interface ICurrentUserService
    {
        Guid GetUserId();
        String GetUserEmail();
        Task<UserRolesVM> GetUser();
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager, IUserService userService,
            IMapper mapper)
        {
            _mapper = mapper;
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

        public async Task<UserRolesVM> GetUser()
        {
            var userId = GetUserId();
            var user =  await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _userManager.GetRolesAsync(user);
            var userRoles = _mapper.Map<UserRolesVM>(user);
            userRoles.RolesName = roles.ToList();
            return userRoles;
                
        }
    }
}
