using AutoMapper;
using MediatR;
using Univali.Api.Entities;
using Univali.Api.Models;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Customers.Commands.CreateCustomerWithAddresses;

public class CreateCustomerWithAddressesCommandHandler : IRequestHandler<CreateCustomerWithAddressesCommand, CreateCustomerWithAddressesDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateCustomerWithAddressesCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CreateCustomerWithAddressesDto> Handle(CreateCustomerWithAddressesCommand request, CancellationToken cancellationToken)
    {
        List<Address> AddressesEntity = request.Addresses
            .Select(address =>
                _mapper.Map<Address>(address)
                ).ToList();

        var customerEntity = _mapper.Map<Customer>(request);
        customerEntity.Addresses = AddressesEntity;

        _customerRepository.AddCustomer(customerEntity);
        await _customerRepository.SaveChangesAsync();
       
        List<AddressDto> addressesDto = customerEntity.Addresses
            .Select(address =>
                _mapper.Map<AddressDto>(address)
                ).ToList();

        var customerToReturn =  _mapper.Map<CreateCustomerWithAddressesDto>(customerEntity);
        customerToReturn.Addresses = addressesDto;
        
        return customerToReturn;
    }
}