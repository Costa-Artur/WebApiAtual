using AutoMapper;
using MediatR;
using Univali.Api.Entities;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Authors.Commands.CreateAuthor;

public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, CreateAuthorDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public CreateAuthorCommandHandler(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public  async Task<CreateAuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorEntity = _mapper.Map<Author>(request);
        _authorRepository.AddAuthor(authorEntity);
        await _authorRepository.SaveChangesAsync();
        var authorToReturn = _mapper.Map<CreateAuthorDto>(authorEntity);
        return authorToReturn;

    }
}