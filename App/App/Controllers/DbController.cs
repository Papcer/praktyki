using App.Models;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public DbController(MongoDbService mongoDbService) => _mongoDbService = mongoDbService;

    [HttpGet]
    public async Task<List<Customer>> GetCustomer() => await _mongoDbService.GetCustomerAsync();

    [HttpGet("Get/{id}")]
    public async Task<Customer> Get(string id)
    {
        var customer = await _mongoDbService.GetCustomerAsyncById(id);
        if(customer is null)
        {
            return null;
        }

        return customer;
    }
    [HttpPost("CreateAsync")]
    public async Task<IActionResult> CreateAsync(Customer customer)
    {
        await _mongoDbService.CreateCustomerAsync(customer);

        return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
    }
    
   
}
