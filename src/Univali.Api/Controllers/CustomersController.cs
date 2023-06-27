using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Univali.Api.DbContexts;
using Univali.Api.Entities;
using Univali.Api.Features.Customers.Commands.CreateCustomer;
using Univali.Api.Features.Customers.Commands.CreateCustomerWithAddresses;
using Univali.Api.Features.Customers.Commands.DeleteCustomer;
using Univali.Api.Features.Customers.Commands.UpdateCustomer;
using Univali.Api.Features.Customers.Commands.UpdateCustomerWithAddresses;
using Univali.Api.Features.Customers.Queries.GetCustomerDetail;
using Univali.Api.Models;
using Univali.Api.Repositories;

namespace Univali.Api.Controllers;


[Route("api/customers")]
[Authorize]
public class CustomersController : MainController
{

    private readonly IMapper _mapper;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMediator _mediator;

    public CustomersController(IMapper mapper, ICustomerRepository customerRepository, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customersFromDatabase = await _customerRepository.GetCustomersAsync();
        var customersToReturn = _mapper.Map<IEnumerable<CustomerDto>>(customersFromDatabase);
        return Ok(customersToReturn);
    }

    [HttpGet("{customerId}", Name = "GetCustomerById")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(
        int customerId)
    {
        var getCustomerDetailQuery = new GetCustomerDetailQuery{Id = customerId};
        var customerToReturn = await  _mediator.Send(getCustomerDetailQuery);

        if(customerToReturn == null) return NotFound();
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
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        CreateCustomerCommand createCustomerCommand
        )
    {
        var customerToReturn = await _mediator.Send(createCustomerCommand);

        return CreatedAtRoute
        (
            "GetCustomerById",
            new { customerId = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCustomer(int id,
        UpdateCustomerCommand customerForUpdateDto
        )
    {
        if (id != customerForUpdateDto.Id) return BadRequest();
        
        var customerUpdate = await  _mediator.Send(customerForUpdateDto);

        if(customerUpdate.sucess == false) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(int id)
    {
        var DeleteCustomer = new DeleteCustomerCommand {Id = id};
        var customerDelete = await _mediator.Send(DeleteCustomer);
        if(customerDelete.sucess == false) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PartiallyUpdateCustomer(
        [FromBody] JsonPatchDocument<CustomerForPatchDto> patchDocument,
        [FromRoute] int id)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerByIdAsync(id);

        if (customerFromDatabase == null) return NotFound();

        var customerToPatch = _mapper.Map<CustomerForPatchDto>(customerFromDatabase);

        patchDocument.ApplyTo(customerToPatch, ModelState);

        if (!TryValidateModel(customerToPatch))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(customerToPatch, customerFromDatabase);

        await _customerRepository.SaveChangesAsync();

        return NoContent();

    }

    [HttpGet("with-addresses")]
    public async Task<ActionResult<IEnumerable<CustomerWithAddressesDto>>> GetCustomersWithAddresses()
    {
        var customersFromDatabase = await _customerRepository.GetCustomersWithAddressesAsync();
        var customersToReturn =  _mapper.Map<IEnumerable<CustomerWithAddressesDto>>(customersFromDatabase);
        return Ok(customersToReturn);
    }

    [HttpGet("with-addresses/{customerId}", Name = "GetCustomerWithAddressesById")]
    public  async Task<ActionResult<CustomerWithAddressesDto>> GetCustomerWithAddressesById(int customerId)
    {
        var customerFromDatabase = await _customerRepository.GetCustomerWithAddressesByIdAsync(customerId);
        if (customerFromDatabase == null) return NotFound();
        var customerToReturn = _mapper.Map<CustomerWithAddressesDto>(customerFromDatabase);
        return Ok(customerToReturn);
    }

    [HttpPost("with-addresses")]
    public async Task<ActionResult<CustomerWithAddressesDto>> CreateCustomerWithAddresses(
       CreateCustomerWithAddressesCommand customerWithAddressesForCreationDto)
    {
      
        var customerToReturn = await _mediator.Send(customerWithAddressesForCreationDto);

        return CreatedAtRoute
        (
            "GetCustomerWithAddressesById",
            new { customerId = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("with-addresses/{customerId}")]
    public async Task<ActionResult> UpdateCustomerWithAddresses(int customerId,
       UpdateCustomerWithAddressesCommand customerWithAddressesForUpdateDto)
    {
        if (customerId != customerWithAddressesForUpdateDto.Id) return BadRequest();

        var updateCustomer = await _mediator.Send(customerWithAddressesForUpdateDto);
        if(updateCustomer.sucess == false) return NotFound();
        return NoContent();
    }

}