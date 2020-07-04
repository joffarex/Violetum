﻿using System.Linq;
using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public class PostHelpers
    {
        public static bool UserOwnsPost(string userId, string postAuthorId)
        {
            return userId == postAuthorId;
        }

        public static IConfigurationProvider GetPostMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostViewModel>()
                    .ForMember(
                        p => p.VoteCount,
                        opt => opt.MapFrom(
                            x => x.PostVotes.Sum(y => y.Direction)
                        )
                    );

                cfg.CreateMap<User, UserBaseViewModel>();
                cfg.CreateMap<Community, PostCommunityViewModel>();
            });
        }

        public static bool IsContentFile(string content)
        {
            string[] dataUri = content.Split(",");

            string[] contentParts = dataUri[0].Split("/");

            return "data".Equals(contentParts[0].Split(":")[0]) && "base64".Equals(contentParts[1].Split(";")[1]);
        }
    }
}