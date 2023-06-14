using AutoMapper;
using MediatR;
using Univali.Api.Entities;
using Univali.Api.Models;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Customers.Commands.UpdateCustomerWithAddresses;

public class UpdateCustomerWithAddressesCommandHandler : IRequestHandler<UpdateCustomerWithAddressesCommand, UpdateCustomerWithAddressesCommandDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public UpdateCustomerWithAddressesCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCustomerWithAddressesCommandDto> Handle(UpdateCustomerWithAddressesCommand request, CancellationToken cancellationToken)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerByIdAsync(request.Id);

        if(customerFromDatabase == null)
        {
            return new UpdateCustomerWithAddressesCommandDto {sucess = false};
        }

        _mapper.Map(request, customerFromDatabase);

        customerFromDatabase.Addresses = request
                                            .Addresses.Select(
                                                address =>
                                                _mapper.Map<Address>(address)
                                            ).ToList();

        await _customerRepository.SaveChangesAsync();

        return new UpdateCustomerWithAddressesCommandDto {sucess = true};
    }
}