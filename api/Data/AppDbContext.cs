using System;
using InventoryManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products {get;set;}
    public DbSet<Category> Categories {get;set;}
    public DbSet<User> Users {get;set;}
}
