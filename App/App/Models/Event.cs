namespace App.Models;

public class Event
{
    public int id { get; set; }

    public string title { get; set; }
    public string location { get; set; }
    public int ticket_limit { get; set; }
    
    public  DateTime start_date { get; set; }
    
}