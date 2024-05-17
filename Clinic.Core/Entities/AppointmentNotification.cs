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
    public class AppointmentNotification
    {
        [Key]
        public String Id { get; set; }
        public DateTime SendAt { get; set; }
        public String Messaage { get; set; }   
        public DateTime CreateAt { get; set; }
        public String SendToId { get; set; }
        [ForeignKey("SendToId")]
        public virtual ApplicationUser User { get; set; }
    }
}
