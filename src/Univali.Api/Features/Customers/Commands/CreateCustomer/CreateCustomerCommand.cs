using System.ComponentModel.DataAnnotations;
using MediatR;
using Univali.Api.ValidationAttributes;

namespace Univali.Api.Features.Customers.Commands.CreateCustomer;

public abstract class CreateCustomerCommand : IRequest<CreateCustomerDto>
{
    [Required(ErrorMessage = "You should fill out a Name")]
    [MaxLength(100, ErrorMessage = "The name shouldn't have more than 100 characters")]
    public string Name {get; set;} = string.Empty;
    [Required(ErrorMessage = "You should fill out a Cpf")]
    /* VALIDACAO ANTIGA --------------------------------------------------
    [StringLength(11, MinimumLength = 11, ErrorMessage = 
    "The Cpf should have 11 characters")]
    ----------------------------------------------------------------------*/
    [CpfMustBeValid(ErrorMessage = "The provided {0} must be valid")]
    public string Cpf {get; set;} = string.Empty;
}