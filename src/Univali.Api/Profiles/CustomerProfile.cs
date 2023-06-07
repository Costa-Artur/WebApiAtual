using AutoMapper;

namespace Univali.Api.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile ()
    {
        /*
            1 arg - Objeto de origem
            2 arg - Objeto de destino
            Mapeia através os nomes das propriedades
            Se a propriedade não existir é ignorada
        */
        CreateMap<Entities.Customer, Models.CustomerDto>();
    }
}