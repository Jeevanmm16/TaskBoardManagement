using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using TaskBoardManagement.Data;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(AppDbContext context, ITokenRepository tokenRepository)
        {
            _context = context;
            _tokenRepository = tokenRepository;
        }

        // ✅ Register
        [HttpPost("register")]
       // [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null)
            {
                return BadRequest("Invalid RoleId. Allowed values are Admin=1, Manager=2, Developer=3.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (userExists)
            {
                return BadRequest("User with this email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // ✅ secure hash
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);

            // Auto-create empty profile
            _context.UserProfiles.Add(new UserProfile
            {
                UserId = user.Id,
                FullName = string.Empty
            });

            await _context.SaveChangesAsync();

            return Ok("User registered successfully. Please login.");
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null) return Unauthorized("Invalid email or password.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordValid) return Unauthorized("Invalid email or password.");

            var token = _tokenRepository.CreateJwtToken(user, user.Role.Name);

            return Ok(new LoginResponseDto { JwtToken = token });
        }
    }
}
