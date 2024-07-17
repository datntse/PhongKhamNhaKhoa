using Clinic.Core.Entities;
using Clinic.Infracstruture.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Infracstructure.Repositories
{
    public interface IRoleRepository : IBaseRepository<IdentityRole, Guid>
    {

    }
    public class RoleRepository : BaseRepository<IdentityRole, Guid>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
