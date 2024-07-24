using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;


namespace Clinic.Infracstruture.Repositories
{
    public interface IClinicRepository : IBaseRepository<ClinicDental, Guid>
    {

    }
    public class ClinicRepository : BaseRepository<ClinicDental, Guid>, IClinicRepository
    {
        public ClinicRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
