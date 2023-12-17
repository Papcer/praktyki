// using App.Helper;
// using App.Models;
// using Microsoft.Extensions.Options;
// using MongoDB.Driver;
//
// namespace App.Services;
//
// public class MongoDbService
// {
//     private readonly IMongoCollection<Customer> _CustomerCollection;
//     private readonly IMongoCollection<Event> _EventCollection;
//     private readonly IMongoCollection<Ticket> _TicketCollection;
//
//     public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
//     {
//         var mongoClient = new MongoClient(
//             mongoDbSettings.Value.ConnectionURL);
//
//         var mongoDatabase = mongoClient.GetDatabase(
//             mongoDbSettings.Value.DatabaseName);
//
//         _CustomerCollection = mongoDatabase.GetCollection<Customer>("test");
//         _EventCollection = mongoDatabase.GetCollection<Event>("Event");
//         _TicketCollection = mongoDatabase.GetCollection<Ticket>("Ticket");
//     }
//
//     //----Customer----//
//     public async Task<List<Customer>> GetCustomerAsync() => await _CustomerCollection.Find(_ => true).ToListAsync();
//
//     public async Task<Customer> GetCustomerAsyncById(string id) =>
//         await _CustomerCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
//
//     public async Task CreateCustomerAsync(Customer customer) => await _CustomerCollection.InsertOneAsync(customer);
//     
//     //----Event----//
//     public async Task<List<Event>> GetEventAsync() => await _EventCollection.Find(_ => true).ToListAsync();
//
// }