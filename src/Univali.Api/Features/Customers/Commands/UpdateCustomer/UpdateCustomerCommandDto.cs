namespace Univali.Api.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
}