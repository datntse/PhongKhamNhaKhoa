using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Clinic.API.Controllers
{
    [Route("api/dentist")]
    [ApiController]
    public class DentistsController : ControllerBase
    {
        private readonly IDentistInfoService _dentistInfoService;

        public DentistsController(IDentistInfoService dentistInfoService)
        {
            _dentistInfoService = dentistInfoService;
        }

        // GET: api/Dentists
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dentists = await _dentistInfoService.GetAll();
            return Ok(dentists);
        }

        // GET: api/Dentists/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(String id)
        {
            var dentist = await _dentistInfoService.FindAsync(id);
            if (dentist == null)
            {
                return NotFound();
            }
            return Ok(dentist);
        }

        // POST: api/Dentists
        [HttpPost("dentistSignup")]
        public async Task<IActionResult> Create([FromBody] DentistSignUp dentistDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _dentistInfoService.DentistSignUp(dentistDTO);
            return Ok(result);
        }

        // PUT: api/Dentists/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(String id, [FromBody] UpdateDentist updateDentist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _dentistInfoService.UpdateDentistInfo(id, updateDentist);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(String id)
        {
            var result = await _dentistInfoService.DeleteDentist(id);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("registerAppointment")]
        public async Task<IActionResult> RegisterAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            var result = await _dentistInfoService.RegisterAppointment(appointmentDTO);
            return Ok(result);
        }


     }
}
