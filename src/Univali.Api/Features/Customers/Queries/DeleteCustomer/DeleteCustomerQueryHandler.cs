using AutoMapper;
using MediatR;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Customers.Queries.DeleteCustomer;

public class DeleteCustomerQueryHandler : IRequestHandler<DeleteCustomerQuery>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerQueryHandler (ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task Handle(DeleteCustomerQuery request, CancellationToken cancellationToken)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerByIdAsync(request.Id);
        if(customerFromDatabase != null) _customerRepository.DeleteCustomer(customerFromDatabase);
        await _customerRepository.SaveChangesAsync();
    }
}