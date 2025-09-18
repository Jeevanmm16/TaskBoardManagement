using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ✅ Require JWT for all profile actions
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _profileRepo;

        public UserProfileController(IUserProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        // ✅ Get logged-in user's profile
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = GetLoggedInUserId();

            var profile = await _profileRepo.GetByUserIdAsync(userId);
            if (profile == null) return NotFound(new { error = "Profile not found" });

            var dto = new UserProfileDto
            {
                UserId = profile.UserId,
                FullName = profile.FullName,
                Phone = profile.Phone,
                Address = profile.Address
            };

            return Ok(dto);
        }

        // ✅ Update logged-in user's profile
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = GetLoggedInUserId();

            var profile = new UserProfile
            {
                UserId = userId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address
            };

            var updated = await _profileRepo.UpdateAsync(profile);
            if (updated == null) return NotFound(new { error = "Profile not found to update" });

            var result = new UserProfileDto
            {
                UserId = updated.UserId,
                FullName = updated.FullName,
                Phone = updated.Phone,
                Address = updated.Address
            };

            return Ok(result);
        }

        // ✅ Extract logged-in userId from JWT
        private  Guid GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("UserId claim missing in token");

            return Guid.Parse(userIdClaim);
        }


        [HttpPatch("me")]
        public async Task<IActionResult> PatchMyProfile([FromBody] PatchUserProfileDto dto)
        {
            var userId = GetLoggedInUserId();

            var profile = await _profileRepo.GetByUserIdAsync(userId);
            if (profile == null) return NotFound();

            // Apply only provided fields
            if (!string.IsNullOrEmpty(dto.FullName))
                profile.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.Phone))
                profile.Phone = dto.Phone;

            if (!string.IsNullOrEmpty(dto.Address))
                profile.Address = dto.Address;

            var updated = await _profileRepo.UpdateAsync(profile);
            if (updated == null) return NotFound();

            var result = new UserProfileDto
            {
                UserId = updated.UserId,
                FullName = updated.FullName,
                Phone = updated.Phone,
                Address = updated.Address
            };
            return Ok(result);
        }
    }
}
