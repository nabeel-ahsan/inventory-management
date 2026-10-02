namespace InventoryManagement.Api.DTOs;

public record ProductQueryParameters(
    string? SearchTerm = null,
    string? SortBy = null,
    string? SortOrder = "asc",
    int PageNumber = 1,
    int PageSize = 10,
    Guid? FilterId = null
);

