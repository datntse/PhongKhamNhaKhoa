using Clinic.Core.Models;
using Clinic.Infracstructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.API.Controllers
{
    [Route("api/clinic")]
    [ApiController]
    public class ClinicDentalsController : ControllerBase
    {
        private readonly IClinicDetailsService _clinicDetailsService;

        public ClinicDentalsController(IClinicDetailsService clinicDetailsService)
        {
            _clinicDetailsService = clinicDetailsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clinics = await _clinicDetailsService.GetAll();

            if (clinics == null || !clinics.Any())
            {
                return NotFound("No clinics found.");
            }

            return Ok(clinics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(String id)
        {
            var clinics = await _clinicDetailsService.FindAsync(id);

            if (clinics == null)
            {
                return NotFound("No clinics found.");
            }

            return Ok(clinics);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClinicDental([FromBody] ClinicDTO clinicDto)
        {
            if (clinicDto == null)
            {
                return BadRequest("Clinic details are required.");
            }

            try
            {
                var createdClinic = await _clinicDetailsService.CreateClinicDental(clinicDto);
                return CreatedAtAction(nameof(GetAll), new { id = createdClinic.Id }, createdClinic);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClinicDental(String id, [FromBody] UpdateClinic updateClinic)
        {
            if (updateClinic == null)
            {
                return BadRequest("Clinic details are required.");
            }

            try
            {
                await _clinicDetailsService.UpdateClinicDental(id, updateClinic);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinicDental(String id)
        {
            var result = await _clinicDetailsService.DeleteClinic(id);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }
    }
}
