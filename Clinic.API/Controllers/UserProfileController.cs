using Clinic.Core.Entities;
using Clinic.Infracstructure.Services;
using Clinic.Infracstruture.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Clinic.API.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private readonly IUserService _userService;

        public UserProfileController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _userService.GetAll();
            return Ok(result);
        }

        [HttpGet("profile/{id}")]
        public IActionResult Get(string id)
        {
            Expression<Func<ApplicationUser, bool>> filter = cd => cd.Id == id;
            var result = _userService.Get(filter).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingUser = _userService.Get(cd => cd.Id == id).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound();
            }

            _userService.Update(user);
            var success = await _userService.SaveChangeAsync();
            if (success)
            {
                return NoContent();
            }
            return BadRequest("Could not update the profile.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Expression<Func<ApplicationUser, bool>> filter = cd => cd.Id == id;
            var existingUser = _userService.Get(filter).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userService.Remove(filter);
            var success = await _userService.SaveChangeAsync();
            if (success)
            {
                return NoContent();
            }
            return BadRequest("Could not delete the profile.");
        }
    }
}
