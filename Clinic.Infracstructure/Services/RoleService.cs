using AutoMapper;
using Clinic.Core.Constants;
using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Clinic.Infracstruture.Repositories;
using Clinic.Infracstruture.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Services
{
    public interface IRoleService
    {
        Task<List<RoleVM>> GetListRole();
        Task<IdentityRole> GetRoleById(String id);
        Task<IdentityResult> CreateRole(String roleName);
        Task<bool> UpdateRole(String roleName, String id);
        Task<IdentityResult> DeleteRole(String roleName);
        Task<IdentityResult> AddRoleUser(List<string> roleNames, String userId);
        Task<String[]> GetUserRole(String userId);
    }

    public class RoleSerivce : IRoleService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userSerivce;

        public RoleSerivce(IUserRepository userRepository, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration configuration,
            RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork, IMapper mapper,
            IRoleRepository roleRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _userSerivce = userService;
        }
     

        public async Task<IQueryable<UserRolesVM>> GetAll()
        {
            var listUserRolesVM = new List<UserRolesVM>();
            var listUser = _userRepository.GetAll().ToList();
            foreach (var user in listUser.ToList())
            {
                var userRoles = (await _userSerivce.GetRolesAsync(user));
                if (userRoles.Contains(AppRole.Admin))
                {
                    listUser.Remove(user);
                }
                else
                {
                    var userRolesVM = _mapper.Map<UserRolesVM>(user);
                    userRolesVM.RolesName = userRoles.ToList();
                    listUserRolesVM.Add(userRolesVM);
                }
            }
            return listUserRolesVM.AsQueryable();
        }

        public async Task<List<RoleVM>> GetListRole()
        {
            var roles = await _roleManager.Roles.OrderBy(_ => _.Name).ToListAsync();
            List<string> roleName = new List<string>();
            if (roles.Count > 0)
            {
                var result = roles.Select(_ => new RoleVM
                {
                    Id = _.Id,
                    Name = _.Name
                });
                return result.ToList();
            }
            return null;
        }

        public async Task<IdentityRole> GetRoleById(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> CreateRole(string roleName)
        {
            IdentityRole _roleName = new IdentityRole(roleName);
            return await _roleManager.CreateAsync(_roleName);
        }

        public async Task<bool> UpdateRole(string roleName, string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                role.Name = roleName;
                _roleRepository.Update(role);
                return await _unitOfWork.SaveChangeAsync();
            }
            return false;
        }

        public async Task<IdentityResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                return await _roleManager.DeleteAsync(role);
            }
            return new IdentityResult();
        }

        public async Task<IdentityResult> AddRoleUser(List<string> roleNames, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = (await _userManager.GetRolesAsync(user)).ToArray<string>();
                var deleteRoles = userRoles.Where(r => !roleNames.Contains(r));
                var addRoles = roleNames.Where(r => !userRoles.Contains(r));
                return await _userManager.RemoveFromRolesAsync(user, deleteRoles);
            }
            return new IdentityResult();
        }

        public async Task<String[]> GetUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = (await _userManager.GetRolesAsync(user)).ToArray<string>();
                return userRoles;
            }
            return null;
        }
    }
}
