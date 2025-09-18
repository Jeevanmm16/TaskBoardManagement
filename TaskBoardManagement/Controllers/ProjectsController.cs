using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
   // [Authorize] // All endpoints require auth
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectMemberRepository _projectMemberRepo;
        private readonly IMapper _mapper;

        public ProjectsController(
            IProjectRepository projectRepo,
            IProjectMemberRepository projectMemberRepo,
            IMapper mapper)
        {
            _projectRepo = projectRepo;
            _projectMemberRepo = projectMemberRepo;
            _mapper = mapper;
        }

        // ------------------ Project CRUD ------------------

        [HttpGet]

        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _projectRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null) return NotFound();

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Manager")] // Only Admins/Managers can create
        public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto dto)
        {
            var project = _mapper.Map<Project>(dto);
            project.Id = Guid.NewGuid();

            var created = await _projectRepo.AddAsync(project);
          
            var result = _mapper.Map<ProjectDto>(created);

            return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto)
        {
            var project = await _projectRepo.GetByIdAsync(id);
          

            _mapper.Map(dto, project);
            await _projectRepo.UpdateAsync(project);

            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> PatchProject(Guid id, PatchProjectDto dto)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null) return NotFound();

            // Apply only provided fields
            if (dto.Name is not null) project.Name = dto.Name;
            if (dto.Description is not null) project.Description = dto.Description;
            if (dto.OwnerId.HasValue)
            {
                // validate new owner
                var ownerExists = await _projectRepo.OwnerExistsAsync(dto.OwnerId.Value);
                if (!ownerExists)
                {
                    return BadRequest($"Owner with Id {dto.OwnerId.Value} does not exist.");
                }
                project.OwnerId = dto.OwnerId.Value;
            }
            // 👉 If dto.OwnerId is not provided → keep existing OwnerId

            await _projectRepo.UpdateAsync(project);
            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // Only Admins can delete
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var deleted = await _projectRepo.DeleteAsync(id);
            if (deleted == null) return NotFound();

            return Ok(_mapper.Map<ProjectDto>(deleted));
        }

        // ------------------ Project Members ------------------

        [HttpGet("{id:guid}/members")]
        public async Task<ActionResult<IEnumerable<ProjectMemberDto>>> GetMembers(Guid id)
        {
            var members = await _projectMemberRepo.GetMembersAsync(id);
            var dtos = members.Select(m => new ProjectMemberDto
            {
                UserId = m.Id,
                Email = m.Email,
                RoleName = m.Role.Name
            });

            return Ok(dtos);
        }

        [HttpPost("{id:guid}/members")]
        [Authorize(Roles = "Admin,Manager")]
       
        public async Task<IActionResult> AddMember(Guid id, AddProjectMemberDto dto)
        {
            var member = await _projectMemberRepo.AddMemberAsync(id, dto.UserId);
            return CreatedAtAction(nameof(GetMembers), new { id }, new { message = "Member added successfully" });
        }


        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            var success = await _projectMemberRepo.RemoveMemberAsync(id, userId);
          
            return Ok(new { message = "Member removed successfully" });
        }
    }
}
