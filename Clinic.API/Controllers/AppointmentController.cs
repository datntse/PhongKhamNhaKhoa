using Clinic.Core.Constants;
using Clinic.Core.Models;
using Clinic.Infracstructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [Route("api/appointment")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("getByUserId")]
        public async Task<IActionResult> GetAppointmentsByUserId(String userId)
        {
            try
            {
                var appointments = await _appointmentService.GetAppointmentsByUserId(userId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting appointments.");
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetAppointmentById(string id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentById(id);
                if (appointment == null)
                {
                    return NotFound("Appointment not found.");
                }
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting appointment.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointment(AppointmentDTO appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.CreateAppointment(appointmentDto);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating appointment.");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAppointment(string id, AppointmentDTO appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointment(id, appointmentDto);
                if (appointment == null)
                {
                    return NotFound("Appointment not found.");
                }
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating appointment.");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAppointment(string id)
        {
            try
            {
                var result = await _appointmentService.DeleteAppointment(id);
                if (!result)
                {
                    return NotFound("Appointment not found.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting appointment.");
            }
        }
    }
}
