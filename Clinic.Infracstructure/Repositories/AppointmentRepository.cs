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

    public interface IAppointmentRepository : IBaseRepository<Appointment, Guid>
    {

    }
    public class AppointmentRepository : BaseRepository<Appointment, Guid>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
