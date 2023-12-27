using AutoMapper;
using ComplexCRUDApplication.Dtos;
using ComplexCRUDApplication.Models;

namespace ComplexCRUDApplication.Helper
{
    public class MappingHandler : Profile
    {
        public MappingHandler() {
            CreateMap<TblCustomer, CustomerDto>().ForMember(dest => dest.StatusName, act => act.MapFrom(src => (src.IsActive) ? "Active" : "Inactive"));
            CreateMap<CustomerDto, TblCustomer>().ForMember(dest => dest.IsActive, act => act.MapFrom(src => (src.StatusName.Trim().ToLower() == "active") ? true : false));
        }
    }
}
