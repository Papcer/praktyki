using App.Models;
using Microsoft.EntityFrameworkCore;
namespace App.Context;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Customer>()
            .ToTable("aplikacja_userdata") 
            .Property(u => u.UserId)
            .HasColumnName("user_id"); // Mapuje właściwość 'LastName' do kolumny 'lastName'
    }
    
    
    //-----User-----//
    //tabela i model
    public DbSet<User> aplikacja_user { get; set; }
    
    
    //-----Customer-----//
    public DbSet<Customer> aplikacja_userdata { get; set; }

}