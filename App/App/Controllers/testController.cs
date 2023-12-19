using App.Context;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class testController : ControllerBase
{
    private readonly ApplicationContext _context;

    public testController(ApplicationContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Api pobierające dane logowania klienta z bazy
    /// </summary>
    /// <response code="200">Dane zostały poprawnie pobrane</response>
    /// <response code="500">Bład podczas pobierania danych</response>
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.aplikacja_user.ToListAsync();
    }
    
    /// <summary>
    /// Api dodające użytkownika do bazy
    /// </summary>
    /// <response code="200">Dane zostały poprawnie dodane</response>
    /// <response code="500">Bład podczas dodawania</response>
    [HttpPost("AddUser")]
    public IActionResult AddEntity([FromBody] User user)
    {
        if (user == null)
        {
            return BadRequest("Invalid data");
        }

        try
        {
            _context.aplikacja_user.Add(user);
            _context.SaveChanges();
            return Ok("Dane dodane poprawnie");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił pewien problem : {ex.Message}");
        }
    }
    
    // Customer //
    /// <summary>
    /// Api pobierające dane klienta z bazy
    /// </summary>
    /// <response code="200">Dane zostały poprawnie pobrane</response>
    /// <response code="500">Bład podczas pobierania danych</response>
    [HttpGet("GetCustomer")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
    {
        return await _context.aplikacja_userdata.ToListAsync();
    }
    
    /// <summary>
    /// Api dodające informację o użytkowniku do bazy
    /// </summary>
    /// <response code="200">Dane zostały poprawnie dodane</response>
    /// <response code="500">Bład podczas dodawania</response>
    [HttpPost("AddCustomer")]
    public IActionResult AddEntity([FromBody] Customer customer)
    {
        if (customer == null)
        {
            return BadRequest("Invalid data");
        }

        try
        {
            _context.aplikacja_userdata.Add(customer);
            _context.SaveChanges();
            return Ok("Dane dodane poprawnie");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił pewien problem : {ex.Message}");
        }
    }
    
    // Events //
    /// <summary>
    /// Api pobierające dane o wydarzeniach z bazy
    /// </summary>
    /// <response code="200">Dane zostały poprawnie pobrane</response>
    /// <response code="500">Bład podczas pobierania danych</response>
    [HttpGet("GetEvents")]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        return await _context.aplikacja_event.ToListAsync();
    }
    
    /// <summary>
    /// Api dodające wydarzenie do bazy danych
    /// </summary>
    /// <response code="200">Dane zostały poprawnie dodane</response>
    /// <response code="500">Bład podczas dodawania</response>
    [HttpPost("AddEvent")]
    public IActionResult AddEntity([FromBody] Event ev)
    {
        if (ev == null)
        {
            return BadRequest("Invalid data");
        }

        try
        {
            _context.aplikacja_event.Add(ev);
            _context.SaveChanges();
            return Ok("Dane dodane poprawnie");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił pewien problem : {ex.Message}");
        }
    }
    
    // Tickets //
    /// <summary>
    /// Api pobierające dane o bilecie z bazy
    /// </summary>
    /// <response code="200">Dane zostały poprawnie pobrane</response>
    /// <response code="500">Bład podczas pobierania danych</response>
    [HttpGet("GetTickets")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        return await _context.aplikacja_ticket.ToListAsync();
    }
    
    /// <summary>
    /// Api dodające bilety do bazy danych
    /// </summary>
    /// <response code="200">Dane zostały poprawnie dodane</response>
    /// <response code="500">Bład podczas dodawania</response>
    [HttpPost("AddTicket")]
    public IActionResult AddEntity([FromBody] Ticket ticket)
    {
        if (ticket == null)
        {
            return BadRequest("Invalid data");
        }

        try
        {
            _context.aplikacja_ticket.Add(ticket);
            _context.SaveChanges();
            return Ok("Dane dodane poprawnie");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Wystąpił pewien problem : {ex.Message}");
        }
    }
    
}