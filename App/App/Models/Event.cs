using System.Text.Json.Serialization;

namespace App.Models;

public class Event
{
    [JsonIgnore]
    public int id { get; set; }
    /// <summary>
    /// Nazwa wydarzenia
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// Pełna lokacja wydarzenia
    /// </summary>
    public string location { get; set; }
    /// <summary>
    /// Limit biletów na wydarzenie
    /// </summary>
    public int ticket_limit { get; set; }
    /// <summary>
    /// Data odbycia wydarzenia
    /// </summary>
    public  DateTime start_date { get; set; }
    
}