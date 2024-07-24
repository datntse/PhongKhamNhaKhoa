using AutoMapper;
using Clinic.Core.Entities;
using Clinic.Core.Models;

namespace Clinic.API.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            #region User
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserRolesVM>().ReverseMap();

            CreateMap<UserDTO, DentistSignUp>().ReverseMap(); 
            CreateMap<DentistDTO, DentistSignUp>().ReverseMap();
            CreateMap<UserRoles, UserRolesVM>().ReverseMap();


            CreateMap<AppointmentDTO, Appointment>().ReverseMap();

            #endregion
        }
    }
}
