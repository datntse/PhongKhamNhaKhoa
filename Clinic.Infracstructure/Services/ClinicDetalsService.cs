using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Services
{
    public interface IClinicDetailsService
    {
        Task<IQueryable<ClinicDental>> GetAll();
        Task AddAsync(ClinicDental clinic);
        Task<ClinicDental> FindAsync(String id);
        void Update(ClinicDental x);
        Task<bool> Remove(String id);
        Task<ClinicDental> CreateClinicDental(ClinicDTO clinicDto);
        Task<IdentityResult> DeleteClinic(String id);

        Task UpdateClinicDental(String id, UpdateClinic updateClinic);
    }

    public class ClinicDetailsService : IClinicDetailsService
    {
        private readonly IClinicDentalsRepository _clinicRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ClinicDetailsService(IClinicDentalsRepository clinicRepository, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _clinicRepository = clinicRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IQueryable<ClinicDental>> GetAll()
        {
            var listClinic = _clinicRepository.GetAll();

            if (listClinic == null || !listClinic.Any())
            {
                return Enumerable.Empty<ClinicDental>().AsQueryable();
            }

            return await Task.FromResult(listClinic);
        }

        public void Update(ClinicDental x)
        {
            _clinicRepository.Update(x);
        }
        public async Task<ClinicDental> FindAsync(String id)
        {
            return await _clinicRepository.FindAsync(id);
        }
        public async Task AddAsync(ClinicDental clinic)
        {
            await _clinicRepository.AddAsync(clinic);
        }

        public async Task<bool> Remove(String id)
        {
            return await _clinicRepository.Remove(id);
        }
        public async Task<ClinicDental> CreateClinicDental(ClinicDTO clinicDto)
        {
            var checkClinic = _clinicRepository.GetAll().ToList();
            checkClinic.FirstOrDefault(c=>c.Address == clinicDto.Address || c.Name == clinicDto.Name);
            if(checkClinic.Any())
            {
                throw new InvalidOperationException("A clinic with the same name or address already exists.");
            }
            var clinicDental = new ClinicDental
            {
                Id = Guid.NewGuid().ToString(),
                Name = clinicDto.Name,
                Address = clinicDto.Address,
                OpenTime = clinicDto.OpenTime,
                CloseTime = clinicDto.CloseTime,
                SlotDuration = clinicDto.SlotDuration,
                MaxPatientsPerSlot = clinicDto.MaxPatientsPerSlot,
                MaxTreatmentPerSlot = clinicDto.MaxTreatmentPerSlot,
                CreateAt = clinicDto.CreateAt,
                Status = 1,
                OnwerId = clinicDto.OnwerId 
            };

            await _clinicRepository.AddAsync(clinicDental);
            await _unitOfWork.SaveChangeAsync();

            return clinicDental;
        }

        public async Task UpdateClinicDental(String id, UpdateClinic updateClinic)
        {
            var existingClinic = await _clinicRepository.FindAsync(id);
            if (existingClinic == null)
            {
                throw new KeyNotFoundException("Clinic not found.");
            }

            existingClinic.Name = updateClinic.Name;
            existingClinic.Address = updateClinic.Address;
            existingClinic.OpenTime = updateClinic.OpenTime;
            existingClinic.CloseTime = updateClinic.CloseTime;
            existingClinic.SlotDuration = updateClinic.SlotDuration;
            existingClinic.MaxPatientsPerSlot = updateClinic.MaxPatientsPerSlot;
            existingClinic.MaxTreatmentPerSlot = updateClinic.MaxTreatmentPerSlot;
            existingClinic.UpdateAt = DateTime.Now;
            existingClinic.Status = updateClinic.Status;
            existingClinic.OnwerId = updateClinic.OnwerId;

            _clinicRepository.Update(existingClinic);

            await _unitOfWork.SaveChangeAsync();
           
        }
        public async Task<IdentityResult> DeleteClinic(string id)
        {
            var dentist = await _clinicRepository.Get(c => c.Id == id).FirstOrDefaultAsync();
            if (dentist == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Dentist not found." });
            }

            _clinicRepository.Remove(dentist);

            var saveResult = await _unitOfWork.SaveChangeAsync();
            if (saveResult)
            {
                return IdentityResult.Success;
            }

            return IdentityResult.Failed(new IdentityError { Description = "Could not delete dentist." });
        }
    }
}
