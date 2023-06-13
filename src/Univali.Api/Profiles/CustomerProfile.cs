using AutoMapper;
using Univali.Api.Entities;
using Univali.Api.Features.Customers.Commands.CreateCustomer;
using Univali.Api.Features.Customers.Commands.CreateCustomerWithAddresses;
using Univali.Api.Features.Customers.Commands.UpdateCustomer;
using Univali.Api.Features.Customers.Commands.UpdateCustomerWithAddresses;
using Univali.Api.Features.Customers.Queries.GetCustomerDetail;
using Univali.Api.Models;

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

        CreateMap<Models.CustomerForCreationDto, Models.CustomerDto>();
        CreateMap<Models.CustomerForUpdateDto, Entities.Customer>();
        CreateMap<Models.CustomerForPatchDto, Entities.Customer>();
        CreateMap<Entities.Customer, Models.CustomerForPatchDto>();

        CreateMap<Entities.Customer, Models.CustomerWithAddressesDto>();    
        CreateMap<Models.CustomerForCreationDto, Entities.Customer>();
        CreateMap<Models.CustomerWithAddressesForCreationDto, Entities.Customer>();
        CreateMap<Models.CustomerWithAddressesForUpdateDto, Entities.Customer>();


        CreateMap<Customer, GetCustomerDetailDto>();
        CreateMap<GetCustomerDetailDto, Customer>();


        CreateMap<CreateCustomerCommand, Customer>();
        CreateMap<Customer, CreateCustomerDto>();
        CreateMap<CustomerForCreationDto, CreateCustomerCommand>();

        CreateMap<UpdateCustomerCommand, Customer>();
        CreateMap<Customer, UpdateCustomerCommandDto>();


        CreateMap<CreateCustomerWithAddressesCommand, Customer>();
        CreateMap<Customer, CreateCustomerWithAddressesDto>();
        CreateMap<CustomerWithAddressesForCreationDto, CreateCustomerWithAddressesCommand>();

        CreateMap<UpdateCustomerWithAddressesCommand, Customer>();
        CreateMap<Customer, UpdateCustomerWithAddressesCommandDto>();
        CreateMap<CustomerWithAddressesForUpdateDto, UpdateCustomerWithAddressesCommandDto>();
    }
}