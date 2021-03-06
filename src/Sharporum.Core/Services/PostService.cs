using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Sharporum.Core.Dtos.Post;
using Sharporum.Core.Helpers;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Specifications;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.CustomExceptions;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Infrastructure;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IAsyncRepository<Community> _communityRepository;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<Post> _postRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IVoteRepository _voteRepository;

        public PostService(IAsyncRepository<Post> postRepository, IVoteRepository voteRepository,
            IAsyncRepository<Community> communityRepository, IUserRepository userRepository,
            UserManager<User> userManager, IMapper mapper)
        {
            _postRepository = postRepository;
            _voteRepository = voteRepository;
            _communityRepository = communityRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PostViewModel> GetPostByIdAsync(string postId)
        {
            var post = await _postRepository.GetByConditionAsync<PostViewModel>(x => x.Id == postId,
                PostHelpers.GetPostMapperConfiguration());
            Guard.Against.NullItem(post, nameof(post));

            return post;
        }

        public async Task<Post> GetPostEntityAsync(string postId)
        {
            Post post = await _postRepository.GetByConditionAsync(x => x.Id == postId);
            Guard.Against.NullItem(post, nameof(post));

            return post;
        }

        public async Task<IEnumerable<PostViewModel>> GetPostsAsync(PostSearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user, nameof(user));

            Community community =
                await _communityRepository.GetByConditionAsync(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            var specification = new PostFilterSpecification(searchParams);

            return await _postRepository.ListAsync<PostViewModel>(specification,
                PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<IEnumerable<PostViewModel>> GetNewsFeedPosts(string userId, PostSearchParams searchParams)
        {
            Community community =
                await _communityRepository.GetByConditionAsync(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            searchParams.Followers = await _userRepository.ListUserFollowingsAsync(userId);

            var specification = new PostFilterSpecification(searchParams);

            return await _postRepository.ListAsync<PostViewModel>(specification,
                PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<int> GetPostsCountAsync(PostSearchParams searchParams)
        {
            Community community =
                await _communityRepository.GetByConditionAsync(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user.Id, nameof(user));

            var specification = new PostFilterSpecification(searchParams);

            return await _postRepository.GetTotalCountAsync(specification);
        }

        public async Task<int> GetPostsCountInNewsFeed(string userId, PostSearchParams searchParams)
        {
            Community community =
                await _communityRepository.GetByConditionAsync(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            searchParams.Followers = await _userRepository.ListUserFollowingsAsync(userId);

            var specification = new PostFilterSpecification(searchParams);

            return await _postRepository.GetTotalCountAsync(specification);
        }

        public async Task<string> CreatePostAsync(string userId, CreatePostDto createPostDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            var post = _mapper.Map<Post>(createPostDto);
            post.AuthorId = user.Id;
            post.ContentType = "application/text";

            await _postRepository.CreateAsync(post);

            return post.Id;
        }

        public async Task<Post> CreatePostWithFileAsync(string userId, CreatePostWithFileDto createPostWithFileDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            var post = _mapper.Map<Post>(createPostWithFileDto);
            post.AuthorId = user.Id;

            await _postRepository.UpdateAsync(post);

            return post;
        }

        public async Task<PostViewModel> UpdatePostAsync(Post post, UpdatePostDto updatePostDto)
        {
            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            await _postRepository.UpdateAsync(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task DeletePostAsync(Post post)
        {
            await _postRepository.DeleteAsync(post);
        }

        public async Task VotePostAsync(string postId, string userId, PostVoteDto postVoteDto)
        {
            try
            {
                User user = await _userManager.FindByIdAsync(userId);
                Guard.Against.NullItem(user, nameof(user));

                Post post = await _postRepository.GetByConditionAsync(x => x.Id == postId);
                Guard.Against.NullItem(post, nameof(post));

                var postVote = await _voteRepository.GetEntityVoteAsync<PostVote>(
                    x => (x.PostId == post.Id) && (x.UserId == user.Id),
                    x => x
                );

                if (postVote != null)
                {
                    postVote.Direction = postVote.Direction == postVoteDto.Direction ? 0 : postVoteDto.Direction;

                    await _voteRepository.UpdateEntityVoteAsync(postVote);
                }
                else
                {
                    var newPostVote = new PostVote
                    {
                        PostId = post.Id,
                        UserId = user.Id,
                        Direction = postVoteDto.Direction,
                    };

                    await _voteRepository.VoteEntityAsync(newPostVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }
    }
}