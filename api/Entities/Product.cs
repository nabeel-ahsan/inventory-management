using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Api.Entities;

public class Product
{
    public Guid Id{get; private set;}
    public string Name{get;set;}
    public string Description{get;set;}
    // 🟢 This tells SQL Server to use decimal with 18 total digits and 2 decimal places
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price{get;set;}
    public int StockQuantity{get;set;}
    public Guid CategoryId{get;set;}
    public Category Category{get;set;}
    public DateTime CreatedAt{get;set;}
    public bool IsActive{get;set;}

    public Product(string name, string description, decimal price, int stockQuantity, Guid categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description=description;
        Price = price;
        StockQuantity = stockQuantity;
        CategoryId = categoryId;
        CreatedAt = DateTime.Today;
        IsActive = true;
    }
}
