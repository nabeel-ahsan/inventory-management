using System;
using InventoryManagement.Api.Data;
using InventoryManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterUser(User user)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(existing => existing.Email == user.Email);

        if(existingUser != null) return null;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User> GetByEmail(string email)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u=>u.Email == email);

        if(existingUser == null) return null;
        
        return existingUser;
    }
}
