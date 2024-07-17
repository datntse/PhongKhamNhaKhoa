using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Models
{
    public class DentistDTO
    {
        public String Description { get; set; }
        public String LicenseNumber { get; set; }
        public int YearOfExperience { get; set; }
        public String ClinicDentalId { get; set; }
    }

    public class UpdateDentist
    {
        public String Description { get; set; }
        public String LicenseNumber { get; set; }
        public int YearOfExperience { get; set; }
        public String ClinicDentalId { get; set; }
        public int Status { get; set; }
    }
}
