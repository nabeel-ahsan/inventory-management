using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.DTOs;

public class InventoryDto
{
    [Required][Length(1, 10000)]
    public int StockQuantity{get;set;}
}
