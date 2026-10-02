using System;

namespace InventoryManagement.Api.Interfaces;

public interface IAuthRepository
{
    Task<User> RegisterUser(User user);
    Task<User> GetByEmail(string email);
}
