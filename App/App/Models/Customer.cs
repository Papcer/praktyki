using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace App.Models;

[Table("aplikacja_userdata")]
public class Customer
{
    public int id { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string phonenumber { get; set; }

    public string Email => User?.username;
    public int UserId { get; set; }
    public User User { get; set; }
}