namespace InventoryManagement.Api;

public record CreateCategoryDto
{
    public Guid Id{get;set;}
    public string Name{get;set;}
}

public record CategoryResponseDto
{
    public string Name{get;set;}
}
