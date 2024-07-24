using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Clinic.Infracstruture.Repositories
{
    public interface IUserRepository : IBaseRepository<ApplicationUser, String>
    {

    }
    public class UserRepository : BaseRepository<ApplicationUser, String>, IUserRepository
    {
        public UserRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
