using AutoMapper;

namespace Univali.Api.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile ()
    {
        CreateMap<Models.AddressDto, Entities.Address>();
        CreateMap<Entities.Address, Models.AddressDto>();
        CreateMap<Models.AddressForCreationDto, Entities.Address>();
        CreateMap<Models.AddressForUpdateDto, Entities.Address>();
    }
}