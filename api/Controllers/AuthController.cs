using InventoryManagement.Api.DTOs;
using InventoryManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        private readonly IAuthService _service;

        public AuthController(ILogger<AuthController> logger,IAuthService authService)
        {
            _logger = logger;
            _service = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Registering user with email: {Email}", dto.Email);
            try
            {
                var (statusCode, user) = await _service.RegisterUserAsync(dto);

                if (statusCode == 400)
                    return BadRequest("Email and Password are required.");

                if (statusCode == 403)
                    return BadRequest("User with this email already exists.");

                if (statusCode == 201)
                    return Ok(user);

                return Problem("Unable to register the user.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to register user with email: {Email} due to an unexpected error.", dto.Email);
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Attempting login for user with email: {Email}", dto.Email);
            try
            {
                var (statusCode, user) = await _service.LoginUserAsync(dto);

                if (statusCode == 400)
                    return BadRequest("Email and Password are required.");

                if (statusCode == 403)
                    return NotFound("User with this email does not exist. Register first.");

                if (statusCode == 401)
                    return Unauthorized("Invalid email or password.");

                if (statusCode == 201)
                    return Ok(user);

                return Problem("Unable to register the user.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to login user with email: {Email} due to an unexpected error.", dto.Email);
                throw;
            }   
        }
    }
}
