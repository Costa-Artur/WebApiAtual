using Microsoft.EntityFrameworkCore;
using Univali.Api.DbContexts;
using Univali.Api.Entities;

namespace Univali.Api.Repositories;

//Implementa Interface ICustomerRepository
public class CustomerRepository : ICustomerRepository
{
    //Injeção de dependencia de customerContext
    private readonly CustomerContext _context;

    public CustomerRepository(CustomerContext customerContext)
    {
        _context = customerContext;
    }
    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await _context.Customers.OrderBy(c => c.Id).ToListAsync();
    }
}