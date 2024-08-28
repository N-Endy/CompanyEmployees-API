using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CompanyEmployees;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForCtorParam("FullAddress",
            opt => opt.MapFrom(c => $"{c.Address}, {c.Country}"));

        CreateMap<Employee, EmployeeDto>();
    }
}