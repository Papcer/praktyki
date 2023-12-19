using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Models;

public class Ticket
{
    [JsonIgnore]
    public int id { get; set; }
    /// <summary>
    /// Id wskazujace na klienta posiadającego bilet
    /// </summary>
    public int userdata_id { get; set; }
    /// <summary>
    /// Id wskazujące na wydarzenie, którego bilet dotyczy
    /// </summary>
    public int event_id { get; set; }
    
}