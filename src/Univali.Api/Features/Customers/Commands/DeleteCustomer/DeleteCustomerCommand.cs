using MediatR;

namespace Univali.Api.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<DeleteCustomerDto>
{
    public int Id {get;set;}

}