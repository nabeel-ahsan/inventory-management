using System;
using InventoryManagement.Api.DTOs;

namespace InventoryManagement.Api.Interfaces;

public interface IAuthService
{
    Task<(int, AuthResponseDto?)> RegisterUserAsync(RegisterDto dto);
    Task<(int ,AuthResponseDto?)> LoginUserAsync(LoginDto dto);
}
