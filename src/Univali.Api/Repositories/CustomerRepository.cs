using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Univali.Api.DbContexts;
using Univali.Api.Entities;

namespace Univali.Api.Repositories;

//Implementa Interface ICustomerRepository
public class CustomerRepository : ICustomerRepository
{
    //Injeção de dependencia de customerContext
    private readonly CustomerContext _context;
    private readonly IMapper _mapper;

    public CustomerRepository(CustomerContext customerContext, IMapper mapper)
    {
        _context = customerContext;
        _mapper = mapper;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetCustomerByCpfAsync(string customerCpf)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Cpf == customerCpf);
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return await _context.Customers.OrderBy(c => c.Id).ToListAsync();
    }

    public void AddCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() > 0);
    }

    public void DeleteCustomer(Customer customer)
    {
        _context.Customers.Remove(customer);
    }

    public async Task<IEnumerable<Customer>> GetCustomersWithAddressesAsync ()
    {
        return await _context.Customers.OrderBy(c => c.Id).Include(c => c.Addresses).ToListAsync();
    }

    public async Task<Customer?> GetCustomerWithAddressesByIdAsync (int customerId)
    {
        return await _context.Customers.Include(c => c.Addresses).FirstOrDefaultAsync(c => c.Id == customerId);
    }

}