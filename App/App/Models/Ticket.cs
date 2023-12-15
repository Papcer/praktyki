using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Models;

public class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public ObjectId CustomerId { get; set; }
    public ObjectId EventId { get; set; }
    
    public string Status { get; set; }
}