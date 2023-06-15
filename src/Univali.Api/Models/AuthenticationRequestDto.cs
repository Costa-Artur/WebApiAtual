using System.ComponentModel.DataAnnotations;

namespace Univali.Api.Models;

public class AuthenticationRequestDto
{
    
    [Required(ErrorMessage = "You should fill out a Name")]
    [MaxLength(50, ErrorMessage = "The name shouldn't have more than 50 characters")]
    public string? Username{get;set;}
    
    [Required(ErrorMessage = "You should fill out a Name")]
    public string? Password {get;set;}
}