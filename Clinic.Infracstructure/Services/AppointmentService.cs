using AutoMapper;
using Clinic.Core.Constants;
using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
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

        Task AddAsync(Appointment appointment);
        IQueryable<Appointment> Get(Expression<Func<Appointment, bool>> where);
        IQueryable<Appointment> Get(Expression<Func<Appointment, bool>> where, Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>> include = null);
        IQueryable<Appointment> Get(Expression<Func<Appointment, bool>> where, params Expression<Func<Appointment, object>>[] includes);
        IQueryable<Appointment> GetAll();
        Task Remove(Expression<Func<Appointment, bool>> where);
        Task<bool> SaveChangeAsync();
        void Update(Appointment clinicDental);


        Task<List<Appointment>> GetAllAppointmentByStatus(int status);
        Task<List<Appointment>> GetAllAppointmentByDate(DateTime dateTime);
        Task<List<Appointment>> GetAll_AppointmentOfDentistById(string dentistId, int status);
        Task<List<Appointment>> GetAll_DentistAppointmentByStatus(int status, string dentitstId);
        Task<List<Appointment>> GetAll_DentistAppointmentByDate(DateTime dateTime, string dentistId);
        Task<List<Dentist>> GetAllDentist_HaveAppointmentAvailableByDate(DateTime dateTime);
        Task<List<Appointment>> GetAll_DentistAppointmentAvailableByDate(DateTime dateTime, string dentistId);
        Task<Appointment> ApproveAppointment(string appoinmentId);
        Task<Appointment> RejectAppointment(string appoinmentId);

        Task<Appointment> CanceltAppointment(string appoinmentId);
        Task<Appointment> FinishAppointment(string appoinmentId);

        Task<List<Appointment>> GetAll_CustomerAppointmentById(string customerId);
        Task<List<Appointment>> GetAll_CustomerAppointmentByDate(DateTime dateTime, string customerId);

        Task<Appointment> BookAppointment(BookAppointment bookAppointment);



    }
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentRepository _appointmentRepository;
        private IClinicDentalsRepository _clinicRepository;
        private IDentistInfoRepository _dentistRepository;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IClinicDentalsRepository clinicRepository,
            IDentistInfoRepository dentistRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _dentistRepository = dentistRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            var clinic = await _clinicRepository.FindAsync(appointmentDto.ClinicId);
            var dentist = await _dentistRepository.FindAsync(appointmentDto.DentistId);

            if (clinic == null || dentist == null)
            {
                throw new Exception("Clinic or dentist not found.");
            }

            // Check if the requested appointment time is available
            // buggggg 
            if (!await IsAppointmentTimeAvailable(appointmentDto.StartAt, appointmentDto.ClinicId, appointmentDto.DentistId))
            {
                throw new Exception("The requested appointment time is not available.");
            }
            var appointment = _mapper.Map<Appointment>(appointmentDto);
            appointment.Status = (int)AppointmentStatus.Booked;

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
            if (!await IsAppointmentTimeAvailable(appointmentDto.StartAt, appointment.ClinicId, appointment.DentistId))
            {
                throw new Exception("The requested appointment time is not available.");
            }

            appointment.StartAt = appointmentDto.StartAt;
            appointment.EndAt = appointmentDto.EndAt;
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

        // cho phép đăng kí trùng lịch dựa trên số bệnh nhân trên mỗi slot.
        private async Task<bool> IsAppointmentTimeAvailable(DateTime date, string clinicId, string dentistId)
        {
            var existingAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.ClinicId == clinicId && a.DentistId == dentistId && a.StartAt == date)
                .CountAsync();

            var clinic = await _clinicRepository.FindAsync(clinicId);
            if (existingAppointments >= clinic.MaxPatientsPerSlot)
            {
                return false;
            }

            return true;
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _appointmentRepository.AddAsync(appointment);
        }

        public IQueryable<Appointment> Get(Expression<Func<Appointment, bool>> where)
        {
            return _appointmentRepository.Get(where);
        }

        public IQueryable<Appointment> Get(Expression<Func<Appointment, bool>> where, Func<IQueryable<Appointment>, IIncludableQueryable<Appointment, object>> include = null)
        {

            return _appointmentRepository.Get(where, include);

        }

        public IQueryable<Appointment> Get(Expression<Func<Appointment, bool>> where, params Expression<Func<Appointment, object>>[] includes)
        {
            return _appointmentRepository.Get(where, includes);
        }

        public IQueryable<Appointment> GetAll()
        {
            return _appointmentRepository.GetAll();
        }

        public async Task Remove(Expression<Func<Appointment, bool>> where)
        {
            await _appointmentRepository.Remove(where);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _unitOfWork.SaveChangeAsync();
        }

        public void Update(Appointment appointment)
        {
            _appointmentRepository.Update(appointment);
        }


        public async Task<List<Appointment>> GetAllAppointmentByStatus(int status)
        {
            return await _appointmentRepository.Get(_ => _.Status == status).ToListAsync();
        }

        public async Task<List<Appointment>> GetAllAppointmentByDate(DateTime dateTime)
        {
            // Normalize the date to remove time part
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var apointments = await _appointmentRepository.GetAll()
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1))
                .ToListAsync();

            return apointments;
        }

        public async Task<List<Appointment>> GetAll_DentistAppointmentByStatus(int status, string dentitstId)
        {
            return await _appointmentRepository.Get(_ => _.Status == status && _.DentistId.Equals(dentitstId)).ToListAsync();
        }

        public async Task<List<Appointment>> GetAll_DentistAppointmentByDate(DateTime dateTime, string dentistId)
        {
            // Normalize the date to remove time part
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var apointments = await _appointmentRepository.GetAll()
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1)
                && a.DentistId.Equals(dentistId))
                .ToListAsync();

            return apointments;
        }

        public async Task<Appointment> ApproveAppointment(string appoinmentId)
        {
            var appointment = await _appointmentRepository.FindAsync(appoinmentId);
            appointment.Status = (int)AppointmentStatus.Approve;
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangeAsync();
            return appointment;
        }

        public async Task<Appointment> RejectAppointment(string appoinmentId)
        {
            var appointment = await _appointmentRepository.FindAsync(appoinmentId);
            appointment.Status = (int)AppointmentStatus.Reject;
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangeAsync();
            return appointment;
        }

        public async Task<List<Dentist>> GetAllDentist_HaveAppointmentAvailableByDate(DateTime dateTime)
        {
            // get danh sahc cac bac si co ngay trong hom nay
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var dentists = await _appointmentRepository.Get(_ => _.Status == (int)AppointmentStatus.Approve, null, _ => _.Dentist)
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1))
                .Select(a => a.Dentist)
                .Distinct()
                .ToListAsync();
            return dentists;
        }

        public async Task<List<Appointment>> GetAll_DentistAppointmentAvailableByDate(DateTime dateTime, string dentistId)
        {
            // get danh sahc cac bac si co ngay trong hom nay
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var appointments = await _appointmentRepository.Get(_ => _.Status == (int)AppointmentStatus.Approve)
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1) && a.DentistId.Equals(dentistId))
                .Distinct()
                .ToListAsync();
            return appointments;
        }

        public async Task<List<Appointment>> GetAll_AppointmentOfDentistById(string dentistId, int status)
        {
            if (status != (int)AppointmentStatus.All)
            {
                var appointments = await _appointmentRepository.Get(_ => _.Status == status)
             .Where(_ => _.DentistId.Equals(dentistId))
             .Distinct()
             .ToListAsync();

                return appointments;
            }
            else
            {
                var appointments = await _appointmentRepository.Get(_ => _.DentistId.Equals(dentistId))
            .Distinct()
            .ToListAsync();

                return appointments;
            }

        }

        public async Task<List<Appointment>> GetAll_CustomerAppointmentById(string customerId)
        {
            // get danh sahc cac bac si co ngay trong hom nay
            var appointments = await _appointmentRepository.Get(_ => _.Status == (int)AppointmentStatus.Approve 
                && _.CustomerId.Equals(customerId))
                .Distinct()
                .ToListAsync();
            return appointments;
        }

        public async Task<List<Appointment>> GetAll_CustomerAppointmentByDate(DateTime dateTime, string customerId)
        {
            // get danh sahc cac bac si co ngay trong hom nay
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var appointments = await _appointmentRepository.Get(_ => _.Status == (int)AppointmentStatus.Approve)
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1) && a.CustomerId.Equals(customerId))
                .Distinct()
                .ToListAsync();
            return appointments;
        }

        public async Task<Appointment> CanceltAppointment(string appoinmentId)
        {
            var appointment = await _appointmentRepository.FindAsync(appoinmentId);
            appointment.Status = (int)AppointmentStatus.Cancel;
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangeAsync();
            return appointment;
        }

        public async Task<Appointment> FinishAppointment(string appoinmentId)
        {
            var appointment = await _appointmentRepository.FindAsync(appoinmentId);
            appointment.Status = (int)AppointmentStatus.Finish;
            _appointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangeAsync();
            return appointment;
        }

        public async Task<Appointment> BookAppointment(BookAppointment bookAppointment)
        {
            if(bookAppointment.AppointmentId != null)
            {
                var appointment = await _appointmentRepository.FindAsync(bookAppointment.AppointmentId);
                appointment.Status = (int)AppointmentStatus.Booked;
                appointment.CustomerId = bookAppointment.CustomerId;
                _appointmentRepository.Update(appointment);
                await _unitOfWork.SaveChangeAsync();
                return appointment;
            }
            return null;
        }
    }
}
