using Univali.Api.Entities;

namespace Univali.Api.Repositories;

public interface ICustomerRepository 
{
    Task<IEnumerable<Customer>> GetCustomersAsync();
    Task<Customer?> GetCustomerByIdAsync(int customerId);
    Task<Customer?> GetCustomerByCpfAsync(string customerCpf);
<<<<<<< HEAD
=======
    void AddCustomer(Customer customer);
    Task<bool> SaveChangesAsync();
>>>>>>> 1e1ec9e60d1c7f18cee5c05fc2fed6dde476ad0d
}