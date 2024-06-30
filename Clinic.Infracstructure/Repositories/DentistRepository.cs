using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Repositories
{
    public interface IDentistRepository : IBaseRepository<Dentist, Guid>
    {
    }
    public class DentistRepository : BaseRepository<Dentist, Guid>, IDentistRepository
    {
        public DentistRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
