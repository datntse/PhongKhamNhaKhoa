using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Models
{
    public class AppointmentDTO
    {
        public String ClinicId { get; set; }
        public String? CustomerId { get; set; }
        public String DentistId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public String Type { get; set; }
        public String Note { get; set; }    
        public int PeriodicInterval { get; set; }
        public int Status { get; set; }
    }
}
