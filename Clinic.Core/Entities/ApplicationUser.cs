using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clinic.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? DOB { get; set; }
        public String FirstName { get; set; }
        public String LastName {  get; set; }
        public String PhoneNumber {  get; set; }
        public String? Address { get; set; }
        public bool Sex { get; set; }
        public int Status {  get; set; }
        public String? AvatarUrl { get; set; }
        public bool IsConfirmEmail { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public String? RefreshToken { get; set; }
        public DateTime? DateExpireRefreshToken { get; set; }
        public virtual Dentist? Dentist { get; set; }
        public virtual ICollection<ClinicDental>? ClinicDentals { get; set; }   
        public virtual ICollection<Appointment>? Appointments { get; set; } 
        public virtual ICollection<AppointmentNotification>? AppointmentNotifications { get; set; } 

    }
}
