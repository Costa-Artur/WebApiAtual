using Univali.Api.Entities;

namespace Univali.Api.Repositories;

public interface IAuthorRepository
{
    void AddAuthor (Author author);
    Task<bool> SaveChangesAsync ();
    Task<Author?> GetAuthorByIdAsync (int authorId);
}