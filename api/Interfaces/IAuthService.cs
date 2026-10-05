using System;
using InventoryManagement.Api.DTOs;

namespace InventoryManagement.Api.Interfaces;

public interface IAuthService
{
    Task<(int, RegisterResponseDto?)> RegisterUserAsync(RegisterDto dto);
    Task<(int ,LoginResponseDto?)> LoginUserAsync(LoginDto dto);
}
