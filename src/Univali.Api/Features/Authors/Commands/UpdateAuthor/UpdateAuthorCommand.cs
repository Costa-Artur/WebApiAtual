using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Univali.Api.Features.Authors.Commands.UpdateAuthor;

public class UpdateAuthorCommand : IRequest<UpdateAuthorDto>
{
    [Required(ErrorMessage = "You should fill out a Id")]
    public int AuthorId {get;set;}
    [Required(ErrorMessage = "You should fill out a First Name")]
    [MaxLength(50, ErrorMessage = "The first name shouldn't have more than 50 characters")]
    public string FirstName {get;set;} = string.Empty;
    [Required(ErrorMessage = "You should fill out a Last Name")]
    [MaxLength(100, ErrorMessage = "The last name shouldn't have more than 100 characters")]
    public string LastName {get;set;} = string.Empty;
}