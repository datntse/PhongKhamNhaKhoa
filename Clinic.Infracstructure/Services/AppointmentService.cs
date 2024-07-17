using Clinic.Core.Constants;
using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Clinic.Infracstructure.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByUserId(string userId);
        Task<Appointment> GetAppointmentById(string id);
        Task<Appointment> CreateAppointment(AppointmentDTO appointmentDto);
        Task<Appointment> UpdateAppointment(string id, AppointmentDTO appointmentDto);
        Task<bool> DeleteAppointment(string id);

    }
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentRepository _appointmentRepository;
        private IClinicDentalRepostiroy _clinicRepository;
        private IDentistRepository _dentistRepository;
        private IUnitOfWork _unitOfWork;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IClinicDentalRepostiroy clinicRepository,
            IDentistRepository dentistRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _dentistRepository = dentistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserId(string userId)
        {
            return await _appointmentRepository.GetAll()
                .Where(a => a.CustomerId == userId)
                .ToListAsync();
        }

        public async Task<Appointment> GetAppointmentById(string id)
        {
            return await _appointmentRepository.FindAsync(id);
        }

        public async Task<Appointment> CreateAppointment(AppointmentDTO appointmentDto)
        {
            var clinic = await _clinicRepository.FindAsync(Guid.Parse(appointmentDto.ClinicId));
            var dentist = await _dentistRepository.FindAsync(Guid.Parse(appointmentDto.DentistId));

            if (clinic == null || dentist == null)
            {
                throw new Exception("Clinic or dentist not found.");
            }

            // Check if the requested appointment time is available
            if (!await IsAppointmentTimeAvailable(appointmentDto.Date, appointmentDto.ClinicId, appointmentDto.DentistId))
            {
                throw new Exception("The requested appointment time is not available.");
            }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid().ToString(),
                ClinicId = appointmentDto.ClinicId,
                CustomerId = appointmentDto.CustomerId,
                DentistId = appointmentDto.DentistId,
                Date = appointmentDto.Date,
                Type = appointmentDto.Type,
                PeriodicInterval = appointmentDto.PeriodicInterval,
                Status = (int)AppointmentStatus.Pending,
                CreateAt = DateTime.UtcNow
            };

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangeAsync();

            return appointment;
        }

        public async Task<Appointment> UpdateAppointment(string id, AppointmentDTO appointmentDto)
        {
            var appointment = await _appointmentRepository.FindAsync(id);
            if (appointment == null)
            {
                return null;
            }

            // Check if the requested appointment time is available
            if (!await IsAppointmentTimeAvailable(appointmentDto.Date, appointment.ClinicId, appointment.DentistId))
            {
                throw new Exception("The requested appointment time is not available.");
            }

            appointment.Date = appointmentDto.Date;
            appointment.Type = appointmentDto.Type;
            appointment.PeriodicInterval = appointmentDto.PeriodicInterval;
            appointment.UpdateAt = DateTime.UtcNow;

            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangeAsync();

            return appointment;
        }

        public async Task<bool> DeleteAppointment(string id)
        {
            var appointment = await _appointmentRepository.FindAsync(id);
            if (appointment == null)
            {
                return false;
            }

            _appointmentRepository.Remove(appointment);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }

        private async Task<bool> IsAppointmentTimeAvailable(DateTime date, string clinicId, string dentistId)
        {
            var existingAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.ClinicId == clinicId && a.DentistId == dentistId && a.Date == date)
                .CountAsync();

            var clinic = await _clinicRepository.FindAsync(Guid.Parse(clinicId));
            if (existingAppointments >= clinic.MaxPatientsPerSlot)
            {
                return false;
            }

            return true;
        }

    }
}
