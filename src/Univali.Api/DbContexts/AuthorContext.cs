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

        var author = modelBuilder.Entity<Author>();

        author
            .HasMany(a => a.Courses)
            .WithMany(c => c.Authors)
            .UsingEntity<AuthorCourse>(
                "AuthorsCourses",
                ac => ac.Property(ac => ac.CreatedOn).HasDefaultValueSql("NOW()")
            );

        author
            .Property(a => a.FirstName)
            .HasMaxLength(30)
            .IsRequired();
            
        author
            .Property(a => a.LastName)
            .HasMaxLength(30)
            .IsRequired();
        

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