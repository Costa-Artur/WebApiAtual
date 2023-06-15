using MediatR;

namespace Univali.Api.Features.Customers.Queries.GetCustomerWithAddressesDetail;

public class GetCustomerWithAddressesDetailQuery : IRequest<GetCustomerWithAddressesDetailDto>
{
    public int Id {get;set;}

}