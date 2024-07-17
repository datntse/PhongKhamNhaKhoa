using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Repositories
{
    public interface IDentistInfoRepository : IBaseRepository<Dentist, String>
    {

    }
    public class DentistInfoRepository : BaseRepository<Dentist,String>, IDentistInfoRepository
    {
        public DentistInfoRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
