using AutoMapper;
using Clinic.API.Services;
using Clinic.Core.Constants;
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
        private readonly IDentistInfoService _dentistInfoService;
        private readonly IJwtTokenService _jwtTokenService;

        public AdminController(IJwtTokenService jwtTokenService, 
            IUserService userService,
            ICurrentUserService currentUserService,
            IRoleService roleService,
            IAppointmentService appointmentService,
            IDentistInfoService dentistInfoService)
        {
            _dentistInfoService = dentistInfoService;
            _jwtTokenService = jwtTokenService;
            _userService = userService;
            _roleService = roleService;
            _appointmentService = appointmentService;
            _currentUserService = currentUserService;

        }
        #region roles;

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
        #endregion


        #region appointment

        [HttpGet("getAllAppointmentByStatus/{status}")]
        public async Task<IActionResult> GetAllAppointment(int status = 0)
        {
            var result = await _appointmentService.GetAllAppointmentByStatus(status);
            return Ok(result);
        }

        [HttpGet("getAllDentistAppointmentStatus/{status}")]
        public async Task<IActionResult> GetAllDentitstAppointment(string dentistId, int status = 0)
        {
            var result = await _appointmentService.GetAll_AppointmentOfDentistById(dentistId, status);
            return Ok(result);
        }


        [HttpGet("approveAppointment/{appointmentId}")]
        public async Task<IActionResult> ApproveAppointment(string appointmentId)
        {
            var result = await _appointmentService.ApproveAppointment(appointmentId);
            return Ok(result);
        }

        [HttpGet("rejectAppointment/{appointmentId}")]
        public async Task<IActionResult> rejectAppointment(string appointmentId)
        {
            var result = await _appointmentService.RejectAppointment(appointmentId);
            return Ok(result);
        }

        [HttpGet("getAllAppointmentByDate")]
        public async Task<IActionResult> GetAllPointmentByDate(DateTime datetime)
        {
            var result = await _appointmentService.GetAllAppointmentByDate(datetime);
            return Ok(result);
        }

        [HttpGet("getAllDentistAppointmentByDate")]
        public async Task<IActionResult> GetAllDentitstPointmentByDate(string dentistId, DateTime datetime, int status = 0)
        {
            var result = await _appointmentService.GetAll_DentistAppointmentByDate(datetime, status, dentistId);
            return Ok(result);
        }

        [HttpGet("getAllDentistByDate")]
        public async Task<IActionResult> getAllDentistByDatetime(DateTime datetime)
        {
            var result = await _appointmentService.GetAllDentist_HaveAppointmentAvailableByDate(datetime);
            return Ok(result);
        }

        [HttpGet("getAllDentistAppointmentByDate/{dentistId}")]
        public async Task<IActionResult> getAllDentistAppointmentByDatetime(string dentistId, DateTime datetime)
        {
            var result = await _appointmentService.GetAll_DentistAppointmentAvailableByDate(datetime, dentistId);
            return Ok(result);
        }

        #endregion

    }
}
