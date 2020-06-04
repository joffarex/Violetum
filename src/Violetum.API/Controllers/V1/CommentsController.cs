﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.API.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
{
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ITokenManager _tokenManager;

        public CommentsController(ICommentService commentService, ITokenManager tokenManager)
        {
            _commentService = commentService;
            _tokenManager = tokenManager;
        }

        [HttpGet(ApiRoutes.Comments.GetMany)]
        public async Task<IActionResult> GetMany([FromQuery] CommentSearchParams searchParams)
        {
            IEnumerable<CommentViewModel> comments = await _commentService.GetComments(searchParams);
            int commentsCount = await _commentService.GetTotalCommentsCount(searchParams);

            return Ok(new GetManyResponse<CommentViewModel>
            {
                Data = comments,
                Count = commentsCount,
                Params = new Params {Limit = searchParams.Limit, CurrentPage = searchParams.CurrentPage},
            });
        }

        [HttpPost(ApiRoutes.Comments.Create)]
        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            createCommentDto.AuthorId = userId;

            CommentViewModel comment = await _commentService.CreateComment(createCommentDto);

            return Created(HttpContext.Request.GetDisplayUrl(), new CommentResponse {Comment = comment});
        }

        [HttpGet(ApiRoutes.Comments.Get)]
        public IActionResult Get(string commentId)
        {
            return Ok(new CommentResponse {Comment = _commentService.GetComment(commentId)});
        }

        [HttpPut(ApiRoutes.Comments.Update)]
        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromRoute] string commentId,
            [FromBody] UpdateCommentDto updateCommentDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            CommentViewModel comment = await _commentService.UpdateComment(commentId, userId, updateCommentDto);

            return Ok(new CommentResponse {Comment = comment});
        }

        [HttpDelete(ApiRoutes.Comments.Delete)]
        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete([FromRoute] string commentId)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _commentService.DeleteComment(commentId, userId);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }

        [HttpPost(ApiRoutes.Comments.Vote)]
        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Vote([FromRoute] string commentId, [FromBody] CommentVoteDto commentVoteDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _commentService.VoteComment(commentId, userId, commentVoteDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}