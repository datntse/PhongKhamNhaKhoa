using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Entities
{
    public class ClinicDental
    {
        [Key]
        public String Id { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public int SlotDuration { get; set; }
        public int MaxPatientsPerSlot { get; set; }
        public int MaxTreatmentPerSlot { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public String OnwerId { get; set; }

        [ForeignKey("OnwerId")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; }
        public virtual ICollection<Dentist>? Dentists { get; set; }
    }
}
