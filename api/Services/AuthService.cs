using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _config;

    public AuthService(
        IAuthRepository authRepository,
        ICategoryRepository categoryRepository,
        IPasswordHasher<User> passwordHasher,
        IConfiguration config
    )
    {
        _repository = authRepository;
        _categoryRepository = categoryRepository;
        _passwordHasher = passwordHasher;
        _config = config;
    }

    public async Task<(int, RegisterResponseDto?)> RegisterUserAsync(RegisterDto dto)
    {
        if (dto.Email == null || dto.Password == null)
            return (400, null);

        var newUser = new User { Username = dto.Username, Email = dto.Email, Role = UserRole.User };

        newUser.Password = _passwordHasher.HashPassword(newUser, dto.Password);

        var result = await _repository.RegisterUser(newUser);

        if (result == null)
            return (403, null);

        var token = GenerateJwtToken(newUser);
        var responseDto = new RegisterResponseDto(newUser.Username, newUser.Email);
        return (201, responseDto);
    }

    public async Task<(int ,LoginResponseDto?)> LoginUserAsync(LoginDto dto)
    {
        if (dto.Email == null || dto.Password == null)
            return (400, null);

        var exisitingUser = await _repository.GetByEmail(dto.Email);

        if (exisitingUser == null)
            return (403, null);

        PasswordVerificationResult result;
        try
        {
            result = _passwordHasher.VerifyHashedPassword(
                exisitingUser,
                exisitingUser.Password,
                dto.Password
            );
        }
        catch (FormatException)
        {
            return (401, null);
        }

        if (result == PasswordVerificationResult.Failed)
        {
            return (401, null);
        }
        else
        {
            var token = GenerateJwtToken(exisitingUser);
            var responseDto = new LoginResponseDto(token, exisitingUser.Username, exisitingUser.Email);
            return (201, responseDto);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_config["JwtSettings:ExpiryInMinutes"]!)
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
