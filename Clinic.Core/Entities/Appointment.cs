using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Entities
{
    public class Appointment
    {
        [Key]
        public String Id { get; set; }
        public String ClinicId { get; set; }
        public String CustomerId { get; set; }
        public String DentistId { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public int PeriodicInterval { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
