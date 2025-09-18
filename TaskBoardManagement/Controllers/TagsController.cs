using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;
using TaskBoardManagement.Repositories;

namespace TaskBoardManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepo;
        private readonly IMapper _mapper;

        public TagsController(ITagRepository tagRepo, IMapper mapper)
        {
            _tagRepo = tagRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _tagRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TagDto>>(tags));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TagDto>> GetTag(Guid id)
        {
            var tag = await _tagRepo.GetByIdAsync(id);
            
            return Ok(_mapper.Map<TagDto>(tag));
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto dto)
        {
            var tag = _mapper.Map<Tag>(dto);
            tag.Id = Guid.NewGuid();

            var created = await _tagRepo.AddAsync(tag);
            return CreatedAtAction(nameof(GetTag), new { id = created.Id }, _mapper.Map<TagDto>(created));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTag(Guid id, UpdateTagDto dto)
        {
            var tag = await _tagRepo.GetByIdAsync(id);
           

            _mapper.Map(dto, tag);
            var updated = await _tagRepo.UpdateAsync(tag);
            return Ok(_mapper.Map<TagDto>(updated));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            var deleted = await _tagRepo.DeleteAsync(id);
           

            return Ok(_mapper.Map<TagDto>(deleted));
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult>  PatchTag(Guid id, PatchTagDto dto)
        {
            var exists = await _tagRepo.GetByIdAsync(id);
            if (dto.Name is not null) exists.Name = dto.Name;
            if (dto.ColorHex is not null) exists.ColorHex = dto.ColorHex;

            var updated = await _tagRepo.UpdateAsync(exists);
            return Ok(_mapper.Map<TagDto>(updated));
        }
    }

}
