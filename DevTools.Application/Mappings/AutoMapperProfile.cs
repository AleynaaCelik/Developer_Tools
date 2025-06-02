using AutoMapper;
using DevTools.Application.DTOs.Auth;
using DevTools.Application.DTOs.CodeAnalysis;
using DevTools.Application.DTOs.Project;
using DevTools.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DevTools.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserInfoDto>();

            // Project mappings
            CreateMap<CreateProjectRequestDto, UserProject>();
            CreateMap<UpdateProjectRequestDto, UserProject>();
            CreateMap<UserProject, ProjectDto>();

            // Code Analysis mappings
            CreateMap<CodeAnalysisSession, CodeAnalysisResponseDto>()
                .ForMember(dest => dest.Result, opt => opt.Ignore());

            CreateMap<CodeIssue, CodeIssueDto>();
            CreateMap<CodeSuggestion, CodeSuggestionDto>();
        }
    }
}
