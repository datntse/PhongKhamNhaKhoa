using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Entities
{
    public class Dentist
    {
        [Key]

        public String Id { get; set; }
        public String Description { get; set; }
        public String LicenseNumber { get; set; }
        public int YearOfExperience { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        [ForeignKey("ClinicDentalId")]
        public String ClinicDentalId { get; set; }
        public virtual ClinicDental ClinicDental { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; }  
    }
}
