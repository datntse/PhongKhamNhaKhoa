using AutoMapper;
using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using Clinic.Infracstruture.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Clinic.Infracstructure.Services
{
    public interface IClinicService
    {
        Task AddAsync(ClinicDental clinicDental);
        IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where);
        IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where, Func<IQueryable<ClinicDental>, IIncludableQueryable<ClinicDental, object>> include = null);
        IQueryable<ClinicDental> Get(Expression<Func<ClinicDental, bool>> where, params Expression<Func<ClinicDental, object>>[] includes);
        IQueryable<ClinicDental> GetAll();
        Task Remove(Expression<Func<ClinicDental, bool>> where);
        Task<bool> SaveChangeAsync();
        void Update(ClinicDental clinicDental);
    }

    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClinicService(IClinicRepository clinicRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<ClinicDental> GetAll()
        {
            return _clinicRepository.GetAll();
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

        public async Task AddAsync(ClinicDental clinicDental)
        {
            await _clinicRepository.AddAsync(clinicDental);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _unitOfWork.SaveChangeAsync();
        }

        public void Update(ClinicDental clinicDental)
        {
            _clinicRepository.Update(clinicDental);
        }

        public async Task Remove(Expression<Func<ClinicDental, bool>> where)
        {
           await _clinicRepository.Remove(where);
        }
    }
}
