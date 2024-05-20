using Clinic.Core.Entities;
using Clinic.Infracstructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Clinic.API.Controllers
{
    [Route("api/clinic")]
    [ApiController]
    //[Authorize]
    public class ClinicController : Controller
    {
        private readonly IClinicService _clinicService;

        public ClinicController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _clinicService.GetAll();
            return Ok(result);
        }

        [HttpGet("clinic/{id}")]
        public IActionResult Get(string id)
        {
            Expression<Func<ClinicDental, bool>> filter = cd => cd.Id == id;
            var result = _clinicService.Get(filter).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("addClinic")]
        public async Task<IActionResult> Add(ClinicDental clinicDental)
        {
            await _clinicService.AddAsync(clinicDental);
            var success = await _clinicService.SaveChangeAsync();
            if (success)
            {
                return CreatedAtAction(nameof(Get), new { id = clinicDental.Id }, clinicDental);
            }
            return BadRequest("Could not add the clinic dental.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ClinicDental clinicDental)
        {
            if (id != clinicDental.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingClinic = _clinicService.Get(cd => cd.Id == id).FirstOrDefault();
            if (existingClinic == null)
            {
                return NotFound();
            }

            _clinicService.Update(clinicDental);
            var success = await _clinicService.SaveChangeAsync();
            if (success)
            {
                return NoContent();
            }
            return BadRequest("Could not update the clinic dental.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Expression<Func<ClinicDental, bool>> filter = cd => cd.Id == id;
            var existingClinic = _clinicService.Get(filter).FirstOrDefault();
            if (existingClinic == null)
            {
                return NotFound();
            }

            await _clinicService.Remove(filter);
            var success = await _clinicService.SaveChangeAsync();
            if (success)
            {
                return NoContent();
            }
            return BadRequest("Could not delete the clinic dental.");
        }
    }
}
