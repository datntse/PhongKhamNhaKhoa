using Clinic.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace Clinic.Core.Models
{
    public class UserDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsDentist { get; set; } = false;
        public String? DOB { get; set; }
        [StringLength(10)]
        public String? PhoneNumber { get; set; }
        public String? Address { get; set; }
        public bool Sex { get; set; }
        // default Status = 1;
        public int Status { get; set; } = 1;
    }
    public class UserRoles : ApplicationUser
    {
        public List<string> RolesName { get; set; }
    }

    public class UserRolesVM
    {
        public string Id { get;set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<string> RolesName { get; set; }
        public String? DOB { get; set; }
        public String? PhoneNumber { get; set; }
        public Dentist? Dentist {  get; set; }
        public bool Sex { get; set; }
        public int Status { get; set; } = 1;
    }


    public class UserSignIn
    {
        public String UserName { get; set; }
        public String Password { get; set; }
    }


}
