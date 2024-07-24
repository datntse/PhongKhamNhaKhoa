using AutoMapper;
using Clinic.Core.Constants;
using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Clinic.Infracstruture.Repositories;
using Clinic.Infracstruture.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Services
{
    public interface IDentistInfoService
    {
        Task<IQueryable<Dentist>> GetAll();
        Task AddAsync(Dentist dentist);
        Task<Dentist> FindAsync(String id);
        IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where);
        IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, params Expression<Func<Dentist, object>>[] includes);
        IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, Func<IQueryable<Dentist>, IIncludableQueryable<Dentist, object>> include = null);
        void Update(Dentist x);
        Task<bool> Remove(String id);
        Task<Dentist> CreateDentistInfo(DentistDTO dentistDTO);
        Task<IdentityResult> DeleteDentist(String id);

        Task UpdateDentistInfo(String id, UpdateDentist updateDentist);
        Task<IdentityResult> DentistSignUp(DentistSignUp dentistSignUp);
        Task<Appointment> RegisterAppointment(AppointmentDTO appointmentDTO);
        Task<List<Appointment>> GetAllAppointmentByStatus(int status);
        Task<List<Appointment>> GetAllAppointmentByDate(DateTime dateTime);

        Task<List<Appointment>> GetAllDentistAppointmentByStatus(int status, string dentitstId);
        Task<List<Appointment>> GetAllDentistAppointmentByDate(DateTime dateTime, string dentistId);

        Task<List<Appointment>> GetAllDentistAppointmentAvailableByDate(DateTime dateTime, string dentistId);


        Task<Appointment> ApproveAppointment(string appoinmentId);
        Task<Appointment> RejectAppointment(string appoinmentId);



    }
    public class DentistInfoService : IDentistInfoService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IDentistInfoRepository _dentistInfoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClinicDentalsRepository _clinicDentalsRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public DentistInfoService(IDentistInfoRepository dentistInfoRepository,
            IUnitOfWork unitOfWork, IClinicDentalsRepository clinicDentalsRepository,
            IUserService userService,
            IClinicDentalsRepository clinicRepository,
            IAppointmentRepository appointmentRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userService = userService;
            _dentistInfoRepository = dentistInfoRepository;
            _unitOfWork = unitOfWork;
            _clinicDentalsRepository = clinicDentalsRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task AddAsync(Dentist dentist)
        {
            await _dentistInfoRepository.AddAsync(dentist);
        }

        public async Task<Dentist> CreateDentistInfo(DentistDTO dentistDTO)
        {
            var clinicDentalExists = await _clinicDentalsRepository.FindAsync(dentistDTO.ClinicDentalId);
            if (clinicDentalExists == null)
            {
                throw new KeyNotFoundException("ClinicDentalId does not exist.");
            }

            var dentist = new Dentist
            {
                Id = Guid.NewGuid().ToString(),
                Description = dentistDTO.Description,
                LicenseNumber = dentistDTO.LicenseNumber,
                YearOfExperience = dentistDTO.YearOfExperience,
                Status = 1,
                CreateAt = DateTime.Now,
                ClinicDentalId = dentistDTO.ClinicDentalId
            };

            var dentistUser = await _userService.FindAsync(dentistDTO.UserId);
            dentistUser.Dentist = dentist;

            _userService.Update(dentistUser);

            await _dentistInfoRepository.AddAsync(dentist);
            await _unitOfWork.SaveChangeAsync();

            return dentist;
        }


        public async Task<Dentist> FindAsync(string id)
        {
            var dentist = await _dentistInfoRepository.Get(d => d.Id.Equals(id))
                .Include(d => d.ClinicDental)
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync();

            if (dentist == null)
            {
                throw new KeyNotFoundException("Dentist not found.");
            }

            return dentist;
        }


        public async Task<IQueryable<Dentist>> GetAll()
        {
            var listDentist = _dentistInfoRepository.GetAll()
                .Include(c => c.ClinicDental)
                .Include(c => c.Appointments);

            if (listDentist == null || !listDentist.Any())
            {
                return Enumerable.Empty<Dentist>().AsQueryable();
            }

            return await Task.FromResult(listDentist);
        }

        public async Task<bool> Remove(String id)
        {
            return await _dentistInfoRepository.Remove(id);
        }

        public void Update(Dentist x)
        {
            _dentistInfoRepository.Update(x);
        }

        public async Task UpdateDentistInfo(String id, UpdateDentist updateDentist)
        {
            var existingDentist = await _dentistInfoRepository.FindAsync(id);
            if (existingDentist == null)
            {
                throw new KeyNotFoundException("Dentist not found.");
            }
            existingDentist.Description = updateDentist.Description;
            existingDentist.LicenseNumber = updateDentist.LicenseNumber;
            existingDentist.YearOfExperience = updateDentist.YearOfExperience;
            existingDentist.Status = updateDentist.Status;
            existingDentist.UpdateAt = DateTime.Now;
            existingDentist.ClinicDentalId = updateDentist.ClinicDentalId;


            _dentistInfoRepository.Update(existingDentist);

            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<IdentityResult> DeleteDentist(string id)
        {
            var dentist = await _dentistInfoRepository.Get(c => c.Id == id).FirstOrDefaultAsync();
            if (dentist == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Dentist not found." });
            }

            _dentistInfoRepository.Remove(dentist);

            var saveResult = await _unitOfWork.SaveChangeAsync();
            if (saveResult)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = "Could not delete dentist." });
        }

        public async Task<IdentityResult> DentistSignUp(DentistSignUp dentistSignUp)
        {
            var userDTO = _mapper.Map<UserDTO>(dentistSignUp);
            var dentistDTO = _mapper.Map<DentistDTO>(dentistSignUp);


            var result = await _userService.SignUpAsync(userDTO);
            if (result.Succeeded)
            {
                var userSignined = _userService.Get(_ => _.Email.Equals(userDTO.Email)).FirstOrDefault();
                if (userSignined != null)
                {
                    dentistDTO.UserId = userSignined.Id;
                    var dentistId = await CreateDentistInfo(dentistDTO);
                }
            }
            return result;
        }
        public IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where)
        {
            return _dentistInfoRepository.Get(where);
        }

        public IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, params Expression<Func<Dentist, object>>[] includes)
        {
            return _dentistInfoRepository.Get(where, includes);
        }

        public IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, Func<IQueryable<Dentist>, IIncludableQueryable<Dentist, object>> include = null)
        {
            return _dentistInfoRepository.Get(where, include);
        }

        public async Task<Appointment> RegisterAppointment(AppointmentDTO appointmentDTO)
        {
            var clinic = await _clinicDentalsRepository.FindAsync(appointmentDTO.ClinicId);

            // Check if the requested appointment time is available
            // buggggg 
            if (!await IsAppointmentTimeAvailable(appointmentDTO.StartAt, appointmentDTO.ClinicId, appointmentDTO.DentistId))
            {
                throw new Exception("The requested appointment time is not available.");
            }
            var appointment = _mapper.Map<Appointment>(appointmentDTO);
            appointment.Status = (int)AppointmentStatus.Pending;

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangeAsync();

            return appointment;
        }


        private async Task<bool> IsAppointmentTimeAvailable(DateTime date, string clinicId, string dentistId)
        {
            var existingAppointments = await _appointmentRepository.GetAll()
                .Where(a => a.ClinicId == clinicId && a.DentistId == dentistId && a.StartAt == date)
                .CountAsync();

            var clinic = await _clinicDentalsRepository.FindAsync(clinicId);
            if (existingAppointments >= clinic.MaxPatientsPerSlot)
            {
                return false;
            }

            return true;
        }


        public async Task<List<Appointment>> GetAllAppointmentByStatus(int status)
        {
            return await _appointmentRepository.Get(_ => _.Status == status).ToListAsync();
        }

        public async Task<List<Appointment>> GetAllAppointmentByStatus(int status, string dentistId)
        {
            return await _appointmentRepository.Get(_ => _.Status == status
            && _.DentistId.Equals(dentistId)).ToListAsync();
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

        public async Task<List<Appointment>> GetAllDentistAppointmentByStatus(int status, string dentitstId)
        {
            return await _appointmentRepository.Get(_ => _.Status == status && _.DentistId.Equals(dentitstId)).ToListAsync();
        }

        public async Task<List<Appointment>> GetAllDentistAppointmentByDate(DateTime dateTime, string dentistId)
        {
            // Normalize the date to remove time part
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var apointments = await _appointmentRepository.GetAll()
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1))
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

        public Task<List<Dentist>> GetAllDentistAppointmentAvailableByDate(DateTime dateTime)
        {
            // get danh sahc cac bac si co ngay trong hom nay
            var normalizedDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

            var dentists = await _appointmentRepository.Get()
                .Where(a => a.StartAt >= normalizedDate && a.StartAt < normalizedDate.AddDays(1))
                .Select(a => a.Dentist)
                .Distinct()
                .ToListAsync();

            return Ok(dentists);
        }
    }
}
