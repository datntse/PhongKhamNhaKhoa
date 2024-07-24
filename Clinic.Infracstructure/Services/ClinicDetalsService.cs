using Clinic.Core.Entities;
using Clinic.Core.Models;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Services
{
    public interface IClinicDetailsService
    {
        Task<IQueryable<ClinicDental>> GetAll();
        Task AddAsync(ClinicDental clinic);
        Task<ClinicDental> FindAsync(String id);
		IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where);
		IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where, params Expression<Func<ClinicDental, object>>[] includes);
		IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where, Func<IQueryable<ClinicDental>, IIncludableQueryable<ClinicDental, object>> include = null);
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
			// Ensure that the _clinicRepository.GetAll() method returns an IQueryable<ClinicDental>
			var listClinic = _clinicRepository.GetAll()
											  .Include(c => c.User)
											  .Include(c => c.Appointments)
											  .Include(c => c.Dentists);

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
			var clinic = await _clinicRepository.Get(d => d.Id.Equals(id))
				.Include(d => d.User)
				.Include(d => d.Appointments)
                .Include(d => d.Dentists)
				.FirstOrDefaultAsync();

			if (clinic == null)
			{
				throw new KeyNotFoundException("clinic not found.");
			}

			return clinic;
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
            var clinics = _clinicRepository.GetAll().ToList();
            var checkClinicDuplicated = clinics.FirstOrDefault(c=>c.Address == clinicDto.Address || c.Name == clinicDto.Name);
            if(checkClinicDuplicated != null)
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

		public IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where)
		{
			return _clinicRepository.Get(where);
		}

		public IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where, params Expression<Func<ClinicDental, object>>[] includes)
		{
			return _clinicRepository.Get(where, includes);
		}

		public IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where, Func<IQueryable<ClinicDental>, IIncludableQueryable<ClinicDental, object>> include = null)
		{
			return _clinicRepository.Get(where, include);
		}
	}
}
