using System;

namespace InventoryManagement.Api.DTOs;

public record RegisterDto(string Username, string Email, string Password);
public record LoginDto(string Email, string Password);
public record RegisterResponseDto(string Username, string Email);

public record LoginResponseDto(string Token, string Username, string Email);