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
using Univali.Api.Repositories;

namespace Univali.Api.Controllers;


[Route("api/customers")]
public class CustomersController : MainController
{

    private readonly Data _data;
    private readonly IMapper _mapper;
    private readonly CustomerContext _context;
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(Data data, IMapper mapper, CustomerContext context, ICustomerRepository customerRepository)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customersFromDatabase = await _customerRepository.GetCustomersAsync();
        var customersToReturn = _mapper.Map<IEnumerable<CustomerDto>>(customersFromDatabase);
        return Ok(customersToReturn);
    }

    [HttpGet("{customerId}", Name = "GetCustomerById")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int customerId)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerByIdAsync(customerId);

        if (customerFromDatabase == null) return NotFound();

        var customerToReturn = _mapper.Map<CustomerDto>(customerFromDatabase);
        return Ok(customerToReturn);
    }


    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDto> GetCustomerByCpf(string cpf)
    {
        var customerFromDatabase = _customerRepository.GetCustomerByCpfAsync(cpf);

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

        var customerEntity = _mapper.Map<Customer>(customerForCreationDto);

        _context.Customers.Add(customerEntity);
        _context.SaveChanges();

        var customerToReturn = _mapper.Map<CustomerDto>(customerEntity);
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

        var customerFromDatabase = _context.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        _mapper.Map(customerForUpdateDto, customerFromDatabase);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCustomer(int id)
    {
        var customerFromDatabase = _context.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        _context.Customers.Remove(customerFromDatabase);
        _context.SaveChanges();

        return NoContent();
    }
    [HttpPatch("{id}")]
    public ActionResult PartiallyUpdateCustomer(
        [FromBody] JsonPatchDocument<CustomerForPatchDto> patchDocument,
        [FromRoute] int id)
    {
        var customerFromDatabase = _context.Customers
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        var customerToPatch = _mapper.Map<CustomerForPatchDto>(customerFromDatabase);

        patchDocument.ApplyTo(customerToPatch, ModelState);

        if (!TryValidateModel(customerToPatch))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(customerToPatch, customerFromDatabase);

        _context.SaveChanges();

        return NoContent();

    }

    [HttpGet("with-addresses")]
    public ActionResult<IEnumerable<CustomerWithAddressesDto>> GetCustomersWithAddresses()
    {
        var customersFromDatabase = _context.Customers
            .OrderBy(c => c.Id)
            .Include(c => c.Addresses)
            .ToList();

        var customersToReturn = _mapper.Map<IEnumerable<CustomerWithAddressesDto>>(customersFromDatabase);

        return Ok(customersToReturn);
    }

    [HttpGet("with-addresses/{customerId}", Name = "GetCustomerWithAddressesById")]
    public ActionResult<CustomerWithAddressesDto> GetCustomerWithAddressesById(int customerId)
    {
        // Obtém o primeiro recurso que encontrar com a id correspondente ou retorna null
        var customerFromDatabase = _context
            .Customers
            .Include(c => c.Addresses)
            .FirstOrDefault(c => c.Id == customerId);

        // Verifica se customer foi encontrado
        if (customerFromDatabase == null) return NotFound();

        /*
            Obtém uma lista de dados mapeados de Address para AddressDto
            Select projeta cada item da lista para um novo formato
            ToList transforma os dados em lista
        */
        var addressesDto = customerFromDatabase
            .Addresses.Select(address =>
            _mapper.Map<AddressDto>(address)
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

        /*
            Obtém uma lista de dados mapeados de AddressWithAddressesForCreationDto para Address
            Select projeta cada item da lista para um novo formato
            ToList transforma os dados em lista
        */
        List<Address> AddressesEntity = customerWithAddressesForCreationDto.Addresses
            .Select(address =>
                _mapper.Map<Address>(address)
                ).ToList();

        // Mapeia a instância customerWithAddressesForCreationDto para Customer
        var customerEntity = _mapper.Map<Customer>(customerWithAddressesForCreationDto);
        customerEntity.Addresses = AddressesEntity;

        // Adiciona no Database
        _context.Customers.Add(customerEntity);
        _context.SaveChanges();

        // Obtém uma lista de dados mapeados de Address para AddressDto
        List<AddressDto> addressesDto = customerEntity.Addresses
            .Select(address =>
                _mapper.Map<AddressDto>(address)
                ).ToList();


        // Mapeia a instância Customer para CustomerDto
        var customerToReturn = _mapper.Map<CustomerWithAddressesDto>(customerEntity);
        customerToReturn.Addresses = addressesDto;

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
        if (customerId != customerWithAddressesForUpdateDto.Id) return BadRequest();

        // Obtém o primeiro recurso que encontrar com a id correspondente ou retorna null
        var customerFromDatabase = _context.Customers
            .FirstOrDefault(c => c.Id == customerId);
        // Verifica se customer foi encontrado
        if (customerFromDatabase == null) return NotFound();

        // Atualiza a instância customer no Database
        _mapper.Map(customerWithAddressesForUpdateDto, customerFromDatabase);


        // Obtém o último id de Address
        /*
            Obtém uma lista de dados mapeados de AddressForAddressDto para Address
            Select projeta cada item da lista para um novo formato
            ToList transforma os dados em lista
        */
        customerFromDatabase.Addresses = customerWithAddressesForUpdateDto
                                        .Addresses.Select(
                                            address =>
                                            _mapper.Map<Address>(address)
                                        ).ToList();

        _context.SaveChanges();
        // Retorna um StatusCode 204 No Content
        return NoContent();
    }

}