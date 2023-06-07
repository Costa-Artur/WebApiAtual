using Microsoft.AspNetCore.Mvc;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId}/addresses")]
public class AddressesController : ControllerBase
{

    private readonly Data _data;

    public AddressesController(Data data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    [HttpGet]
    public ActionResult<IEnumerable<AddressDto>> GetAddresses(int customerId)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == customerId);

        if (customerFromDatabase == null) return NotFound();

        var addressesToReturn = new List<AddressDto>();

        foreach (var address in customerFromDatabase.Addresses)
        {
            addressesToReturn.Add(new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City
            });
        }

        return Ok(addressesToReturn);

    }

[HttpGet("{addressId}", Name="GetAddress")]
public ActionResult<AddressDto> GetAddress(int customerId, int addressId)
{
  // Obtém o primeiro Customer que encontrar com a id correspondente ou retorna null
  var customerFromDatabase = _data.Customers
      .FirstOrDefault(customer => customer.Id == customerId);

  // Verifica se Customer foi encontrado
  if (customerFromDatabase == null) return NotFound();

  // Obtém o primeiro Address que encontrar com a id correspondente ou retorna null
  var addressFromDatabase = customerFromDatabase.Addresses
      .FirstOrDefault(address => address.Id == addressId);

  // Verifica se Address foi encontrado
  if (addressFromDatabase == null) return NotFound();

  var addressToReturn = new AddressDto
  {
      Id = addressFromDatabase.Id,
      Street = addressFromDatabase.Street,
      City = addressFromDatabase.City
  };

  // Retorna StatusCode 200 com os Addresses no corpo do response
  return Ok(addressToReturn);
}

[HttpPost]
public ActionResult<AddressDto> CreateAddress(
   int customerId,
   AddressForCreationDto addressForCreationDto)
{
   // Obtém o Customer ou retorna null
   var customerFromDatabase = _data.Customers
       .FirstOrDefault(c => c.Id == customerId);

   // Verifica se Customer existe
   if (customerFromDatabase == null) return NotFound();

   /*
       Obtém o último Id de Address
       SelectMany retorna uma lista com todos endereços de todos usuários
       Max obtém a Id com o valor mais alto
   */
   var maxAddressId = _data.Customers
       .SelectMany(c => c.Addresses).Max(a => a.Id);

   // var addresses = _data.Customers
   //     .SelectMany(c => c.Addresses);
  
   // foreach(var address in addresses)
   // {
   //     Console.WriteLine($"Street: {address.Street}");
   //     Console.WriteLine($"City: {address.City}");
   // }

   // Mapeia a instância AddressForCreationDto para Address
   var addressEntity = new Address()
   {
       Id = ++maxAddressId,
       Street = addressForCreationDto.Street,
       City = addressForCreationDto.City
   };


   // Inseri no Singleton
   customerFromDatabase.Addresses.Add(addressEntity);


   // Mapeia a Instância Address do Singleton para uma instância AddressDto
   var addressToReturn = new AddressDto()
   {
       Id = addressEntity.Id,
       City = addressEntity.City,
       Street = addressEntity.Street
   };


   // Retorna um status code 201 com o local onde o recurso possa ser obtido
   return CreatedAtRoute("GetAddress",
       new
       {
           customerId = customerFromDatabase.Id,
           addressId = addressToReturn.Id
       },
       addressToReturn
   );
}

[HttpPut("{addressId}")]
public ActionResult UpdateAddress(int customerId, int addressId,
   AddressForUpdateDto addressForUpdateDto)
{
   if(addressForUpdateDto.Id != addressId) return BadRequest();

   // Obtém o primeiro Customer que encontrar com a id correspondente ou retorna null
   var customerFromDatabase = _data.Customers
       .FirstOrDefault(c => c.Id == customerId);

   // Verifica se Customer foi encontrado
   if(customerFromDatabase == null) return NotFound();

   // Obtém o primeiro Address que encontrar com a id correspondente ou retorna null
   var addressFromDatabase = customerFromDatabase.Addresses
       .FirstOrDefault(a => a.Id == addressId);

   // Verifica se Address foi encontrado
   if(addressFromDatabase == null) return NotFound();

   // Atualiza Address no Database
   addressFromDatabase.City = addressForUpdateDto.City;
   addressFromDatabase.Street = addressForUpdateDto.Street;

   // Retorna Status Code 204 No Content
   return NoContent();
}

[HttpDelete("{addressId}")]
public ActionResult DeleteAddress(int customerId, int addressId)
{
   var customerFromDatabase = _data.Customers
       .FirstOrDefault(customer => customer.Id == customerId);

   if (customerFromDatabase == null) return NotFound();

   var addressFromDatabase = customerFromDatabase.Addresses
       .FirstOrDefault(address => address.Id == addressId);

   if (addressFromDatabase == null) return NotFound();

   customerFromDatabase.Addresses.Remove(addressFromDatabase);

   return NoContent();
}


}
