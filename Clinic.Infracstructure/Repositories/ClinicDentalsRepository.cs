using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Repositories
{
    public interface IClinicDentalsRepository : IBaseRepository<ClinicDental, String>
    {

    }
    public class ClinicDentalsRepository : BaseRepository<ClinicDental, String>, IClinicDentalsRepository
    {
        public ClinicDentalsRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
