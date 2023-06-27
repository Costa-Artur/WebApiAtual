using AutoMapper;
using MediatR;
using Univali.Api.Repositories;

namespace Univali.Api.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, UpdateAuthorDto>
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public UpdateAuthorCommandHandler(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }
    public async Task<UpdateAuthorDto> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorFromDatabase = await _authorRepository.GetAuthorByIdAsync(request.AuthorId);
        if(authorFromDatabase == null)
        {
            return new UpdateAuthorDto {Success = false};   
        }
        _mapper.Map(request, authorFromDatabase);

        await _authorRepository.SaveChangesAsync();

        return new UpdateAuthorDto {Success = true};
    }
}