using System.ComponentModel.DataAnnotations;

namespace Univali.Api.Models;

public class CustomerForUpdateDto : CustomerForManipulationDto
{
    [Required(ErrorMessage = "You should fill out an Id")]
    public int Id {get; set;}
    public override string Cpf {get; set;} = string.Empty;
}