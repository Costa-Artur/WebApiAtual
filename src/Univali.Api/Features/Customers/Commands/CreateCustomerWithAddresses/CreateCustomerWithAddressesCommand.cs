using System.ComponentModel.DataAnnotations;
using MediatR;
using Univali.Api.Entities;
using Univali.Api.Models;
using Univali.Api.ValidationAttributes;

namespace Univali.Api.Features.Customers.Commands.CreateCustomerWithAddresses;

// IRequest<> transforma a classe em uma Mensagem
// CreateCustomerDto é o tipo que este comando espera receber de volta
public class CreateCustomerWithAddressesCommand : IRequest<CreateCustomerWithAddressesDto>
{
    [Required(ErrorMessage = "You should fill out a Name")]
    [MaxLength(100, ErrorMessage = "The name shouldn't have more than 100 characters")]
    public string Name {get; set;} = string.Empty;

    [Required(ErrorMessage = "You should fill out a Cpf")]
    [CpfMustBeValid(ErrorMessage = "The provided {0} should be valid number.")]
    public string Cpf {get; set;} = string.Empty;
    public ICollection<AddressDto> Addresses {get; set;} = new List<AddressDto>();
}