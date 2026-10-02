using System.ComponentModel.DataAnnotations;
using InventoryManagement.Api.Entities;

namespace InventoryManagement.Api;

public class ProductDto
{
    public Guid Id{get;set;}

    [Required][StringLength(100, MinimumLength =3)]
    public string? Name{get;set;}
    public string? Description{get;set;}
    public decimal? Price{get;set;}
    public int? StockQuantity{get;set;}
    public Guid CategoryId{get;set;}
}

public class UpdateProductDto
{
    [Required][StringLength(100, MinimumLength =3)]
    public string? Name{get;set;}
    public string? Description{get;set;}
    public decimal? Price{get;set;}
    public Guid CategoryId{get;set;}
}
