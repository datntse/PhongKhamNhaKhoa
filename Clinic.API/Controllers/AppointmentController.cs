using Clinic.Core.Constants;
using Clinic.Core.Models;
using Clinic.Infracstructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
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

        [HttpPost("bookAppointment")]
        public async Task<IActionResult> CreateAppointment(BookAppointment BookAppointment)
        {
            try
            {
                var appointment = await _appointmentService.BookAppointment(BookAppointment);
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


        [HttpGet("getCustomerAppointment/{customerId}")]
        public async Task<IActionResult> GetAllDentitstAppointment(string customerId)
        {
            var result = await _appointmentService.GetAll_CustomerAppointmentById(customerId);
            return Ok(result);
        }
        [HttpGet("getCustomerAppointmentByDate/{customerId}")]
        public async Task<IActionResult> GetCustomerAppointmentByDate(string customerId, DateTime dateTime)
        {
            var result = await _appointmentService.GetAll_CustomerAppointmentByDate(dateTime, customerId);
            return Ok(result);
        }

        [HttpGet("getAllDentistAppointmentByDate")]
        public async Task<IActionResult> GetAllDentitstPointmentByDate(string dentistId, DateTime datetime, int status = 0)
        {
            var result = await _appointmentService.GetAll_DentistAppointmentByDate(datetime, status, dentistId);
            return Ok(result);
        }

        [HttpGet("getAllDentistAppointmentStatus/{dentistId}")]
        public async Task<IActionResult> GetAllDentitstAppointment(string dentistId, int status = 0)
        {
            var result = await _appointmentService.GetAll_AppointmentOfDentistById(dentistId, status);
            return Ok(result);
        }
    }
}
