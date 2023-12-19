using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Models;

[Table("aplikacja_userdata")]
public class Customer
{
    [JsonIgnore]
    public int id { get; set; }
    
    /// <summary>
    /// Imię klienta
    /// </summary>
    public string firstName { get; set; }
    /// <summary>
    /// Nazwisko klienta
    /// </summary>
    public string lastName { get; set; }
    /// <summary>
    /// Numer telefonu klienta
    /// </summary>
    public string phoneNumber { get; set; }
    /// <summary>
    /// Email klienta mapowany z tabeli User
    /// </summary>
    public string Email => User?.username;
    /// <summary>
    /// id wskazujące na tabele user
    /// </summary>
    public int UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
}