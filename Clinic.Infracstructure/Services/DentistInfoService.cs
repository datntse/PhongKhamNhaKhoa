using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Clinic.Infracstruture.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Services
{
    public interface IDentistInfoService
    {
        Task<IQueryable<Dentist>> GetAll();
        Task AddAsync(Dentist dentist);
        Task<Dentist> FindAsync(String id);
        void Update(Dentist x);
        Task<bool> Remove(String id);
        Task<Dentist> CreateDentistInfo(DentistDTO dentistDTO);
        Task<IdentityResult> DeleteDentist(String id);

        Task UpdateDentistInfo(String id, UpdateDentist updateDentist);
    }
    public class DentistInfoService : IDentistInfoService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDentistInfoRepository _dentistInfoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClinicDentalsRepository _clinicDentalsRepository;

        public DentistInfoService(IDentistInfoRepository dentistInfoRepository, 
            IUnitOfWork unitOfWork, IClinicDentalsRepository clinicDentalsRepository,
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _dentistInfoRepository = dentistInfoRepository;
            _unitOfWork = unitOfWork;
            _clinicDentalsRepository = clinicDentalsRepository;
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

            var dentistUser = await _userRepository.FindAsync(dentistDTO.UserId);
            dentistUser.Dentist = dentist;

             _userRepository.Update(dentistUser);

            await _dentistInfoRepository.AddAsync(dentist);
            await _unitOfWork.SaveChangeAsync();

            return dentist;
        }


        public async Task<Dentist> FindAsync(String id)
        {
            return await _dentistInfoRepository.FindAsync(id);
        }

        public async Task<IQueryable<Dentist>> GetAll()
        {
            var listDentist = _dentistInfoRepository.GetAll();

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

    }
}
