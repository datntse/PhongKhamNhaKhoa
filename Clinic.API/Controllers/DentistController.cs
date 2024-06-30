using Clinic.Core.Entities;
using Clinic.Infracstructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Clinic.API.Controllers
{
    [Route("api/dentist")]
    [ApiController]
    public class DentistController : Controller
    {
        private readonly IDentistService _dentistService;

        public DentistController(IDentistService dentistService)
        {
            _dentistService = dentistService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _dentistService.GetAll();
            return Ok(result);
        }

        [HttpGet("dentist/{id}")]
        public IActionResult Get(string id)
        {
            Expression<Func<Dentist, bool>> filter = cd => cd.Id == id;
            var result = _dentistService.Get(filter).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("addDentist")]
        public async Task<IActionResult> Add(Dentist dentist)
        {
            await _dentistService.AddAsync(dentist);
            var success = await _dentistService.SaveChangeAsync();
            if (success)
            {
                return CreatedAtAction(nameof(Get), new { id = dentist.Id }, dentist);
            }
            return BadRequest("Could not add the dentist.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Dentist dentist)
        {
            if (id != dentist.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingDentist = _dentistService.Get(cd => cd.Id == id).FirstOrDefault();
            if (existingDentist == null)
            {
                return NotFound();
            }

            _dentistService.Update(dentist);
            var success = await _dentistService.SaveChangeAsync();
            if (success)
            {
                return NoContent();
            }
            return BadRequest("Could not update the dentist.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Expression<Func<Dentist, bool>> filter = cd => cd.Id == id;
            var existingDentist = _dentistService.Get(filter).FirstOrDefault();
            if (existingDentist == null)
            {
                return NotFound();
            }

            await _dentistService.Remove(filter);
            var success = await _dentistService.SaveChangeAsync();
            if (success)
            {
                return NoContent();
            }
            return BadRequest("Could not delete the dentist.");
        }
    }
}
