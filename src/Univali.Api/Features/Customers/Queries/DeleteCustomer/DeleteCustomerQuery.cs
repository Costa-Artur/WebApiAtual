using MediatR;

namespace Univali.Api.Features.Customers.Queries.DeleteCustomer;

public class DeleteCustomerQuery : IRequest
{
    public int Id {get;set;}

}