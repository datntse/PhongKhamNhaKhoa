using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public String UserId { get; set; }
    }

    public class DentistSignUp
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsDentist { get; set; } = true;
        public String? DOB { get; set; }
        public String? PhoneNumber { get; set; }
        public bool Sex { get; set; }
        public int Status { get; set; } = 1;
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
