using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    
    private readonly Data _data;
    private readonly IMapper _mapper;

    public CustomersController(Data data, IMapper mapper)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }


    [HttpGet]
    public ActionResult<IEnumerable<CustomerDto>> GetCustomers()
    {
        var customersFromDatabase = _data.Customers;
        var customersToReturn = _mapper.Map<IEnumerable<CustomerDto>>(customersFromDatabase);
        return Ok(customersToReturn);
    }

    [HttpGet("{id}", Name = "GetCustomerById")]
    public ActionResult<CustomerDto> GetCustomerById(int id)
    {
        var customerFromDatabase = _data
            .Customers.FirstOrDefault(c => c.Id == id);

        if (customerFromDatabase == null) return NotFound();

       var customerToReturn = _mapper.Map<CustomerDto>(customerFromDatabase);
        return Ok(customerToReturn);
    }


    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDto> GetCustomerByCpf(string cpf)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(c => c.Cpf == cpf);

        if (customerFromDatabase == null)
        {
            return NotFound();
        }

       var customerToReturn = _mapper.Map<CustomerDto>(customerFromDatabase);
        return Ok(customerToReturn);
    }

    [HttpPost]
    public ActionResult<CustomerDto> CreateCustomer(
        CustomerForCreationDto customerForCreationDto)
    {

        /*if (!ModelState.IsValid)
        {
            Response.ContentType = "application/problem+json";
            // Cria a fábrica de um objeto de detalhes de problema de validação
            var problemDetailsFactory = HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>();

            // Cria um objeto de detalhes de problema de validação
            var validationProblemDetails = problemDetailsFactory
                .CreateValidationProblemDetails(HttpContext, ModelState);

            // Atribui o status code 422 no corpo do response
            validationProblemDetails.Status = StatusCodes.Status422UnprocessableEntity;

            return UnprocessableEntity(validationProblemDetails);
        }*/

        var customerEntity = new Customer()
        {
            Id = _data.Customers.Max(c => c.Id) + 1,
            Name = customerForCreationDto.Name,
            Cpf = customerForCreationDto.Cpf
        };

        _data.Customers.Add(customerEntity);

        var customerToReturn = _mapper.Map<CustomerDto>(customerForCreationDto);
        customerToReturn.Id = customerEntity.Id;

        return CreatedAtRoute
        (
            "GetCustomerById",
            new { id = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCustomer(int id,
        CustomerForUpdateDto customerForUpdateDto)
    {
        if (id != customerForUpdateDto.Id) return BadRequest();

        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

       _mapper.Map(customerForUpdateDto, customerFromDatabase);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCustomer(int id)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        _data.Customers.Remove(customerFromDatabase);

        return NoContent();
    }
    [HttpPatch("{id}")]
    public ActionResult PartiallyUpdateCustomer(
        [FromBody] JsonPatchDocument<CustomerForPatchDto> patchDocument,
        [FromRoute] int id)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        var customerToPatch = new CustomerForPatchDto
        {
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };

        patchDocument.ApplyTo(customerToPatch);

        _mapper.Map(customerToPatch, customerFromDatabase);

        return NoContent();

    }

    [HttpGet("with-addresses")]
    public ActionResult<IEnumerable<CustomerWithAddressesDto>> GetCustomersWithAddresses()
    {
        var customersFromDatabase = _data.Customers;

        var customersToReturn = customersFromDatabase
            .Select(customer => new CustomerWithAddressesDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Cpf = customer.Cpf,
                Addresses = customer.Addresses
                    .Select(address => new AddressDto
                    {
                        Id = address.Id,
                        City = address.City,
                        Street = address.Street
                    }).ToList()
            });

        return Ok(customersToReturn);
    }

    [HttpGet("with-addresses/{customerId}", Name="GetCustomerWithAddressesById")]
    public ActionResult<CustomerWithAddressesDto> GetCustomerWithAddressesById(int customerId)
    {
        // Obtém o primeiro recurso que encontrar com a id correspondente ou retorna null
        var customerFromDatabase = _data
            .Customers.FirstOrDefault(c => c.Id == customerId);

        // Verifica se customer foi encontrado
        if (customerFromDatabase == null) return NotFound();

        /*
            Obtém uma lista de dados mapeados de Address para AddressDto
            Select projeta cada item da lista para um novo formato
            ToList transforma os dados em lista
        */
        var addressesDto = customerFromDatabase
            .Addresses.Select(address =>
            new AddressDto
            {
                Id = address.Id,
                City = address.City,
                Street = address.Street
            }
        ).ToList();

        // Mapeia Customer para CustomerDto
        var customerToReturn = _mapper.Map<CustomerWithAddressesDto>(customerFromDatabase);
        // Retorna StatusCode 200 com o Customer no corpo do response
        return Ok(customerToReturn);
    }

[HttpPost("with-addresses")]
public ActionResult<CustomerWithAddressesDto> CreateCustomerWithAddresses(
   CustomerWithAddressesForCreationDto customerWithAddressesForCreationDto)
{
   /*
       Obtém o último Id de Address
       SelectMany retorna uma lista com todos endereços de todos usuários
       Max obtém a Id com o valor mais alto
   */
   var maxAddressId = _data.Customers
       .SelectMany(c => c.Addresses).Max(c => c.Id);

   /*
       Obtém uma lista de dados mapeados de AddressWithAddressesForCreationDto para Address
       Select projeta cada item da lista para um novo formato
       ToList transforma os dados em lista
   */
   List<Address> AddressesEntity = customerWithAddressesForCreationDto.Addresses
       .Select(address =>
           new Address
           {
               Id = ++maxAddressId,
               Street = address.Street,
               City = address.City
           }).ToList();

   // Mapeia a instância customerWithAddressesForCreationDto para Customer
   var customerEntity = new Customer
   {
       Id = _data.Customers.Max(c => c.Id) + 1, // Obtém id do customer
       Name = customerWithAddressesForCreationDto.Name,
       Cpf = customerWithAddressesForCreationDto.Cpf,
       Addresses = AddressesEntity // Atribui o Address mapeado
   };


   // Adiciona no Database
   _data.Customers.Add(customerEntity);


   // Obtém uma lista de dados mapeados de Address para AddressDto
   List<AddressDto> addressesDto = customerEntity.Addresses
       .Select(address =>
           new AddressDto
           {
               Id = address.Id,
               Street = address.Street,
               City = address.City
           }).ToList();


   // Mapeia a instância Customer para CustomerDto
   var customerToReturn = new CustomerWithAddressesDto
   {
       Id = customerEntity.Id,
       Name = customerEntity.Name,
       Cpf = customerEntity.Cpf,
       Addresses = addressesDto // Atribui o Address mapeado
   };


   // Retorna uma resposta com o cabeçalho de localização do novo recurso
   return CreatedAtRoute
   (
       // Nome do método
       "GetCustomerWithAddressesById",
       // Objeto anônimo que possui os parâmetros do método
       new { customerId = customerToReturn.Id },
       // O novo registro criado
       customerToReturn
   );
}

[HttpPut("with-addresses/{customerId}")]
public ActionResult UpdateCustomerWithAddresses(int customerId,
   CustomerWithAddressesForUpdateDto customerWithAddressesForUpdateDto)
{
   if(customerId != customerWithAddressesForUpdateDto.Id) return BadRequest();

  // Obtém o primeiro recurso que encontrar com a id correspondente ou retorna null
  var customerFromDatabase = _data.Customers
      .FirstOrDefault(c => c.Id == customerId);
   // Verifica se customer foi encontrado
  if (customerFromDatabase  == null) return NotFound();

  // Atualiza a instância customer no Database
  customerFromDatabase.Name = customerWithAddressesForUpdateDto.Name;
  customerFromDatabase.Cpf = customerWithAddressesForUpdateDto.Cpf;


  // Obtém o último id de Address
  var maxAddressId = _data.Customers
      .SelectMany(c => c.Addresses)
      .Max(c => c.Id);

  /*
      Obtém uma lista de dados mapeados de AddressForAddressDto para Address
      Select projeta cada item da lista para um novo formato
      ToList transforma os dados em lista
  */
  customerFromDatabase.Addresses = customerWithAddressesForUpdateDto
                                  .Addresses.Select(
                                      address =>
                                      new Address()
                                      {
                                          Id = ++maxAddressId,
                                          City = address.City,
                                          Street = address.Street
                                      }
                                  ).ToList();


  // Retorna um StatusCode 204 No Content
  return NoContent();
}



}