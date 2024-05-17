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
            #endregion
        }
    }
}
