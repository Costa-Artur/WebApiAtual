using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Univali.Api.DbContexts;
using Univali.Api.Entities;

namespace Univali.Api.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly AuthorContext _context;
    private readonly IMapper _mapper;

    public AuthorRepository(AuthorContext authorContext, IMapper mapper)
    {
        _context = authorContext;
        _mapper = mapper;
    }

    public void AddAuthor (Author author)
    {
        _context.Add(author);
    }

    public async Task<Author?> GetAuthorByIdAsync(int authorId)
    {
        return await _context.Authors.FirstOrDefaultAsync(c => c.AuthorId == authorId);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() > 0);
    }
}