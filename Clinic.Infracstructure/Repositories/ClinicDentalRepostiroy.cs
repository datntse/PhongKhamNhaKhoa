using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Repositories
{

    public interface IClinicDentalRepostiroy : IBaseRepository<ClinicDental, Guid>
    {

    }
    public class ClinicDentalRepostiroy : BaseRepository<ClinicDental, Guid>, IClinicDentalRepostiroy
    {
        public ClinicDentalRepostiroy(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
