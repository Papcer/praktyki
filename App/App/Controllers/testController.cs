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
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.aplikacja_user.ToListAsync();
    }
    [HttpGet("GetCustomer")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
    {
        return await _context.aplikacja_userdata.ToListAsync();
    }
}