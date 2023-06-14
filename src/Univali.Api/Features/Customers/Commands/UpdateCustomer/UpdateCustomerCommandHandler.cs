using AutoMapper;
using MediatR;
using Univali.Api.Entities;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerCommandDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCustomerCommandDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerByIdAsync(request.Id);
        if(customerFromDatabase == null)
        {
            return new UpdateCustomerCommandDto {sucess = false};
        }
        _mapper.Map(request, customerFromDatabase);

        await _customerRepository.SaveChangesAsync();

        return new UpdateCustomerCommandDto {sucess = true};
    }

}