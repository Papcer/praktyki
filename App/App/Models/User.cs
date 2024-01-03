using System.Text.Json.Serialization;

namespace App.Models;

public class User
{
    [JsonIgnore]
    public int id { get; set; }
    /// <summary>
    /// Email klienta
    /// </summary>
    public string username { get; set; }
    /// <summary>
    /// Hasło klienta
    /// </summary>
    public string password { get; set; }

    public bool email_verified { get; set; }
}