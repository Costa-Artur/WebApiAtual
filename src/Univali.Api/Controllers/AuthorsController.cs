using AutoMapper;
using MediatR;
using Univali.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Univali.Api.Features.Authors.Commands.CreateAuthor;
using Univali.Api.Features.Authors.Commands.UpdateAuthor;

namespace Univali.Api.Controllers;

[Route("api/authors")]
public class AuthorsController : MainController
{
    private readonly IMapper _mapper;
    private readonly IAuthorRepository _authorRepository;
    private readonly IMediator _mediator;

    public AuthorsController(IMapper mapper, IAuthorRepository authorRepository, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<ActionResult<CreateAuthorDto>> CreateAuthor (
        CreateAuthorCommand createAuthorCommand
    )
    {
        var authorToReturn = await _mediator.Send(createAuthorCommand);

        // return CreatedAtRoute
        // (
        //     "GetAuthorById",
        //     new { authorId = authorToReturn.AuthorId },
        //     authorToReturn
        // );

        return Ok(authorToReturn);
    }

    [HttpPut("{id}")]

    public async Task<ActionResult> UpdateAuthor(
        int id,
        UpdateAuthorCommand updateAuthorCommand
    )
    {
        if(updateAuthorCommand.AuthorId != id) return BadRequest();
        var updateCustomer = await _mediator.Send(updateAuthorCommand);

        if(updateCustomer.Success == false) return NotFound();

        return NoContent();
    }

}