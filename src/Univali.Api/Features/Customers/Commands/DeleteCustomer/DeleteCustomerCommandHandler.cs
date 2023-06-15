using AutoMapper;
using MediatR;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, DeleteCustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler (ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<DeleteCustomerDto> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerByIdAsync(request.Id);

        if(customerFromDatabase == null) return new DeleteCustomerDto {sucess = false};

        _customerRepository.DeleteCustomer(customerFromDatabase);
        await _customerRepository.SaveChangesAsync();
        return new DeleteCustomerDto {sucess = true};
    }   

}