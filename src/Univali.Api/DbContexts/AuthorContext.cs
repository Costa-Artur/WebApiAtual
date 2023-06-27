using Microsoft.EntityFrameworkCore;
using Univali.Api.Entities;

namespace Univali.Api.DbContexts;

public class AuthorContext : DbContext
{
    public DbSet<Author> Authors { get; set; } = null!;

    public AuthorContext (DbContextOptions<AuthorContext> options)
        :base (options) { }

    protected override void OnModelCreating (ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasData
            (
                new Author(
                    "Stephen",
                    "King"
                ) {
                    AuthorId = 1
                },
                new Author(
                    "George",
                    "Orwell"
                ){
                    AuthorId = 2
                }
               
            );

        base.OnModelCreating (modelBuilder);
    }

}