using AutoMapper;
using Clinic.API.Services;
using Clinic.Core.Entities;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstructure.Services;
using Clinic.Infracstruture.Data;
using Clinic.Infracstruture.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    //[Authorize]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAppointmentService _appointmentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJwtTokenService _jwtTokenService;

        public AdminController(IJwtTokenService jwtTokenService, 
            IUserService userService,
            ICurrentUserService currentUserService,
            IRoleService roleService,
            IAppointmentService appointmentService)
        {
            _jwtTokenService = jwtTokenService;
            _userService = userService;
            _roleService = roleService;
            _appointmentService = appointmentService;
            _currentUserService = currentUserService;

        }

        [HttpGet("getListRole")]
        public async Task<IActionResult> GetListRole()
        {
            var roles = await _roleService.GetListRole();
            List<string> roleName = new List<string>();
            if (roles.Count > 0)
            {
                var result = roles.Select(_ => new
                {
                    _.Id,
                    _.Name
                });
                return Ok(result);
            }
            return Ok();
        }

        [HttpGet("getRoleBy/{id}")]
        public async Task<IActionResult> GetRoleById(String id)
        {
            var role = await _roleService.GetRoleById(id);
            if (role != null) { 
                return Ok(role);
            } else
            {
                return BadRequest();
            }
        }


        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole(String roleName)
        {
            var result = await _roleService.CreateRole(roleName);
            if(result.Succeeded) {
                return Ok();
            } else 
                return BadRequest();
        }


        [HttpPut("updateRole/{id}")]
        public async Task<IActionResult> UpdateRole(String roleName, String id)
        {
            try
            {
                var result = await _roleService.UpdateRole(roleName, id);
                if(result)
                {
                    return Ok();
                } else return BadRequest();
            }
            catch (Exception)
            {
                 return BadRequest();
            }
        }


        [HttpDelete("deleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(String roleId)
        {
            var result = await _roleService.DeleteRole(roleId);
            if(result.Succeeded)
            {
                return Ok();
            } 
            return BadRequest();
        }


        [HttpGet("getUserRole")]
        public async Task<IActionResult> GetListUsers()
        {
            var result = await _roleService.GetListUsers();
            return Ok(result);

        }

    }
}
