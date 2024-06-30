using AutoMapper;
using Clinic.Core.Entities;
using Clinic.Infracstructure.Repositories;
using Clinic.Infracstruture.Data;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Services
{
    public interface IDentistService
    {
        Task AddAsync(Dentist dentist);
        IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where);
        IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, Func<IQueryable<Dentist>, IIncludableQueryable<Dentist, object>> include = null);
        IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, params Expression<Func<Dentist, object>>[] includes);
        IQueryable<Dentist> GetAll();
        Task Remove(Expression<Func<Dentist, bool>> where);
        Task<bool> SaveChangeAsync();
        void Update(Dentist dentist);
    }

    public class DentistService : IDentistService
    {
        private readonly IDentistRepository _dentistRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DentistService(IDentistRepository dentistRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _dentistRepository = dentistRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<Dentist> GetAll()
        {
            return _dentistRepository.GetAll();
        }

        public IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where)
        {
            return _dentistRepository.Get(where);
        }

        public IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, params Expression<Func<Dentist, object>>[] includes)
        {
            return _dentistRepository.Get(where, includes);
        }

        public IQueryable<Dentist> Get(Expression<Func<Dentist, bool>> where, Func<IQueryable<Dentist>, IIncludableQueryable<Dentist, object>> include = null)
        {
            return _dentistRepository.Get(where, include);
        }

        public async Task AddAsync(Dentist dentist)
        {
            await _dentistRepository.AddAsync(dentist);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _unitOfWork.SaveChangeAsync();
        }

        public void Update(Dentist dentist)
        {
            _dentistRepository.Update(dentist);
        }

        public async Task Remove(Expression<Func<Dentist, bool>> where)
        {
            _dentistRepository.Remove(where);
        }
    }
}
