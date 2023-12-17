using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Models;

public class Ticket
{
    public int id { get; set; }
    
    public int userdata_id { get; set; }
    public int event_id { get; set; }
    
}