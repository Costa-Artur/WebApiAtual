using System.ComponentModel.DataAnnotations;
using Univali.Api.ValidationAttributes;

namespace Univali.Api.Models;

public abstract class CustomerForManipulationDto
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