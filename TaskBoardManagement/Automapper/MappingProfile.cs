using AutoMapper;
using TaskBoardManagement.Models.Domain;
using TaskBoardManagement.Models.DTOs;


namespace TaskBoardManagement.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<PatchUserDto, User>().ReverseMap();
            CreateMap<PatchUserProfileDto, UserProfile>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();
            CreateMap<TaskItem, TaskItemDto>();
            CreateMap<CreateTaskItemDto, TaskItem>();
            CreateMap<UpdateTaskItemDto, TaskItem>();

            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<CreateTagDto, Tag>();
            CreateMap<UpdateTagDto, Tag>();
            CreateMap<TaskTag, TaskTagDto>();

            
           

            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<CreateTagDto, Tag>();
            CreateMap<UpdateTagDto, Tag>();
        }
    }
 }

