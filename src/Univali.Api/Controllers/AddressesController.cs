using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Univali.Api.DbContexts;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;

[Route("api/customers/{customerId}/addresses")]
public class AddressesController : MainController
{
    private readonly Data _data;
    private readonly IMapper _mapper;
    private readonly CustomerContext _context;

    public AddressesController(Data data, IMapper mapper, CustomerContext context)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    public ActionResult<IEnumerable<AddressDto>> GetAddresses(int customerId)
    {
        var customerFromDatabase = _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefault(customer => customer.Id == customerId);
        if (customerFromDatabase == null) return NotFound();

        var addressesToReturn = new List<AddressDto>();

        foreach (var address in customerFromDatabase.Addresses)
        {
            addressesToReturn.Add(
                _mapper.Map<AddressDto>(address)
            );
        }
        return Ok(addressesToReturn);
    }

    [HttpGet("{addressId}", Name = "GetAddress")]
    public ActionResult<AddressDto> GetAddress(int customerId, int addressId)
    {
        // Obtém o primeiro Customer que encontrar com a id correspondente ou retorna null
        var customerFromDatabase = _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefault(customer => customer.Id == customerId);

        // Verifica se Customer foi encontrado
        if (customerFromDatabase == null) return NotFound();

        // Obtém o primeiro Address que encontrar com a id correspondente ou retorna null
        var addressFromDatabase = customerFromDatabase.Addresses
            .FirstOrDefault(address => address.Id == addressId);

        // Verifica se Address foi encontrado
        if (addressFromDatabase == null) return NotFound();

        var addressToReturn = _mapper.Map<AddressDto>(addressFromDatabase);

        // Retorna StatusCode 200 com os Addresses no corpo do response
        return Ok(addressToReturn);
    }

    ///<summary>Creates Address for customer</summary>
    ///<param name="customerId"> Id do customer </param>
    ///<param name="addressForCreationDto"> Dto do endereco </param>
    ///
    [HttpPost]
    public ActionResult<AddressDto> CreateAddress(
       int customerId,
       AddressForCreationDto addressForCreationDto)
    {
        // Obtém o Customer ou retorna null
        var customerFromDatabase = _context.Customers
            .FirstOrDefault(c => c.Id == customerId);

        // Verifica se Customer existe
        if (customerFromDatabase == null) return NotFound();

        /*
            Obtém o último Id de Address
            SelectMany retorna uma lista com todos endereços de todos usuários
            Max obtém a Id com o valor mais alto
        */

        // var addresses = _data.Customers
        //     .SelectMany(c => c.Addresses);

        // foreach(var address in addresses)
        // {
        //     Console.WriteLine($"Street: {address.Street}");
        //     Console.WriteLine($"City: {address.City}");
        // }

        // Mapeia a instância AddressForCreationDto para Address
        var addressEntity = _mapper.Map<Address>(addressForCreationDto);


        // Inseri no Singleton
        customerFromDatabase.Addresses.Add(addressEntity);

        _context.SaveChanges();

        // Mapeia a Instância Address do Singleton para uma instância AddressDto
        var addressToReturn = _mapper.Map<AddressDto>(addressEntity);


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
        if (addressForUpdateDto.Id != addressId) return BadRequest();

        // Obtém o primeiro Customer que encontrar com a id correspondente ou retorna null
        var customerFromDatabase = _context.Customers
             .Include(c => c.Addresses)
            .FirstOrDefault(c => c.Id == customerId);

        // Verifica se Customer foi encontrado
        if (customerFromDatabase == null) return NotFound();

        // Obtém o primeiro Address que encontrar com a id correspondente ou retorna null
        var addressFromDatabase = customerFromDatabase.Addresses
            .FirstOrDefault(a => a.Id == addressId);

        // Verifica se Address foi encontrado
        if (addressFromDatabase == null) return NotFound();

        // Atualiza Address no Database
        _mapper.Map(addressForUpdateDto, addressFromDatabase);

        _context.SaveChanges();

        // Retorna Status Code 204 No Content
        return NoContent();
    }

    [HttpDelete("{addressId}")]
    public ActionResult DeleteAddress(int customerId, int addressId)
    {
        var customerFromDatabase = _context.Customers
             .Include(c => c.Addresses)
            .FirstOrDefault(customer => customer.Id == customerId);

        if (customerFromDatabase == null) return NotFound();

        var addressFromDatabase = customerFromDatabase.Addresses
            .FirstOrDefault(address => address.Id == addressId);

        if (addressFromDatabase == null) return NotFound();

        customerFromDatabase.Addresses.Remove(addressFromDatabase);

        _context.SaveChanges();

        return NoContent();
    }


}
