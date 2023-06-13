using Univali.Api.Entities;

namespace Univali.Api.Repositories;

public interface ICustomerRepository 
{
    Task<IEnumerable<Customer>> GetCustomersAsync();

    Task<Customer?> GetCustomerByIdAsync(int customerId);
    Task<Customer?> GetCustomerByCpfAsync(string customerCpf);
}