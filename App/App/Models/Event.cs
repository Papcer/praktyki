using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace App.Models;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string name { get; set; }
    public string location { get; set; }
    public int ticketLimit { get; set; }
    
    public  DateTime data_time { get; set; }
    
}