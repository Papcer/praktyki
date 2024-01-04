using App.Context;
using App.Models;
using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerateFakeData : Controller
{
    private readonly ApplicationContext _context;
    
    public GenerateFakeData(ApplicationContext context)
    {
        _context = context;
    }
    
    private int GenerateUniqueUserId(HashSet<int> usedIds, List<User> fakeUsers)
    {
        int newUserId;
        do
        {
            var randomUser = fakeUsers[new Random().Next(fakeUsers.Count)];
            newUserId = randomUser.id;
        }while (!usedIds.Add(newUserId));

        return newUserId;
    }
        
    
    /// <summary>
    /// Api generujące użytkowników, wydarzenia oraz dane do tabeli user
    /// </summary>
    /// <response code="200">Dane został dodane do bazy</response>
    /// <response code="400">Bład podczas dodawania</response>
    [HttpGet("AddFakeData")]
    public async Task<ActionResult> addFakeDataToDataBase()
    {
        var usedUserIds = new HashSet<int>();

        var fakerUser = new Faker<User>()
            .UseSeed(System.DateTime.Now.Millisecond)
            .RuleFor(u => u.username, f => f.Person.Email)
            .RuleFor(u => u.password, "test")
            .RuleFor(u => u.email_verified, false);

        var fakeUsers = fakerUser.Generate(100000);
        
        var uniqueUsernames = new HashSet<string>();

        foreach (var user in fakeUsers)
        {
            while (!uniqueUsernames.Add(user.username))
            {
                user.username = fakerUser.Generate().username;
            }
        }
        
        _context.aplikacja_user.AddRange(fakeUsers);
        await _context.SaveChangesAsync();

        var fakerCustomer = new Faker<Customer>()
            .UseSeed(System.DateTime.Now.Millisecond)
            .RuleFor(c => c.firstName, f => f.Person.FirstName)
            .RuleFor(c => c.lastName, f => f.Person.LastName)
            .RuleFor(c => c.Email, f => f.Person.Email)
            .RuleFor(c => c.phoneNumber, f => f.Random.Replace("#########"))
            .RuleFor(c => c.UserId, f => GenerateUniqueUserId(usedUserIds, fakeUsers));
        
        var customers = fakerCustomer.Generate(100000);
        _context.aplikacja_userdata.AddRange(customers);
        await _context.SaveChangesAsync();

        var fakeEvents = new Faker<Event>()
            .UseSeed(System.DateTime.Now.Millisecond)
            .RuleFor(e => e.title, f => f.Random.Word())
            .RuleFor(e => e.location, f => f.Address.FullAddress())
            .RuleFor(e => e.ticket_limit, f => f.Random.Number(100, 1000))
            .RuleFor(e => e.start_date, f => f.Date.Recent().ToUniversalTime()); 
            
        var events = fakeEvents.Generate(250);
        _context.aplikacja_event.AddRange(events);
        await _context.SaveChangesAsync();
        
        foreach (var eventItem in events)
        {
            var fakeTickets = new Faker<Ticket>()
                .UseSeed(System.DateTime.Now.Millisecond)
                .RuleFor(t => t.event_id, f => eventItem.id)
                .RuleFor(t => t.userdata_id, f => f.PickRandom(customers).id);
            
            var ticketsForEvent = fakeTickets.Generate((int)(eventItem.ticket_limit * 0.5));
            _context.aplikacja_ticket.AddRange(ticketsForEvent);
        }
        
        await _context.SaveChangesAsync();
        return Ok("Dane zostały dodane do bazy");
    }
}