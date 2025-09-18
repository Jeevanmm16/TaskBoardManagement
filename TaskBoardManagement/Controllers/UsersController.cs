using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoardManagement.Data;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Require JWT token for all endpoints
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly AppDbContext dbContext;
        private readonly IMapper mapper;

        public UsersController(IUserRepository userRepo, AppDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            _userRepo = userRepo;
        }

        // ✅ Any authenticated user can view users
        [HttpGet]
        public async Task<IActionResult> GetUsers(
             [FromQuery] int page = 1,
             [FromQuery] int pageSize = 10,
             [FromQuery] string? sortBy = null,
             [FromQuery] string sortOrder = "asc",
             [FromQuery] string? search = null)
        {
            var (users, totalItems, error) = await _userRepo.GetAllAsync(page, pageSize, sortBy, sortOrder, search);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var usersDto = mapper.Map<List<UserDto>>(users);

            return Ok(new
            {
                page,
                pageSize,
                totalPages,
                totalItems,
                items = usersDto,
                error
            });
        }

        // ✅ Any authenticated user can view a single user
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.Name
            };
        }

        // ✅ Only Admins can create users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = dto.Password, // ⚠️ will hash later
                RoleId = (int)dto.RoleId
            };

            var createdUser = await _userRepo.AddAsync(user);

            if (createdUser == null)
            {
                return BadRequest($"Invalid RoleId {dto.RoleId}. Allowed values are Admin=1, Manager=2, Developer=3.");
            }

            var result = new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                RoleId = createdUser.RoleId,
                RoleName = createdUser.Role?.Name ?? ""
            };

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, result);
        }

        // ✅ Only Admins can update users
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = dto.Email;
            user.RoleId = dto.RoleId;

            await _userRepo.UpdateAsync(user);
            return NoContent();
        }

        // ✅ Only Admins can delete users
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var deletedUser = await _userRepo.DeleteAsync(id);

            if (deletedUser == null)
            {
                return NotFound();
            }

            var dto = new UserDto
            {
                Id = deletedUser.Id,
                Email = deletedUser.Email,
                RoleId = deletedUser.RoleId,
                RoleName = deletedUser.Role?.Name ?? ""
            };

            return Ok(dto); // return deleted data
        }
        [HttpPatch("{id:guid}")]
        // [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> PatchUser(Guid id, PatchUserDto dto)
        {
            if (dto == null)
                return BadRequest("Patch data cannot be null.");

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Apply only provided fields
            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            // Only update RoleId if it's provided (not zero)
            if (dto.RoleId != 0)
            {
                // Optional: validate role exists
                // var roleExists = await _userRepo.RoleExistsAsync(dto.RoleId);
                // if (!roleExists)
                //     return BadRequest($"Role with Id {dto.RoleId} does not exist.");

                user.RoleId = dto.RoleId;
            }

            // Do NOT allow patching sensitive fields like PasswordHash or Id

            await _userRepo.UpdateAsync(user);
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? ""
            };

            return Ok(userDto);
        }
    }
}
