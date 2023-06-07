using Microsoft.EntityFrameworkCore;
using Univali.Api.Entities;

namespace Univali.Api.DbContexts;

public class CustomerContext : DbContext
{
    public DbSet<Customer> Customers {get; set;} = null!;
    public DbSet<Address> Addresses {get; set;} = null!;

    public CustomerContext(DbContextOptions<CustomerContext> options):base(options){}
}