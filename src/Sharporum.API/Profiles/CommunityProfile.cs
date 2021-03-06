﻿using AutoMapper;
using Sharporum.Core.Dtos.Community;
using Sharporum.Core.ViewModels.Community;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.Entities;

namespace Sharporum.API.Profiles
{
    public class CommunityProfile : Profile
    {
        public CommunityProfile()
        {
            CreateMap<Community, CommunityViewModel>();
            CreateMap<Community, PostCommunityViewModel>();
            CreateMap<CreateCommunityDto, Community>();
        }
    }
}