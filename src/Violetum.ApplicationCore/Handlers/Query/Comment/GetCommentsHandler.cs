﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Comment;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Handlers.Query.Comment
{
    public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, FilteredDataViewModel<CommentViewModel>>
    {
        private readonly ICommentService _commentService;

        public GetCommentsHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<FilteredDataViewModel<CommentViewModel>> Handle(GetCommentsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommentViewModel> comments = await _commentService.GetCommentsAsync(request.SearchParams);
            int commentsCount = await _commentService.GetCommentsCountAsync(request.SearchParams);

            return new FilteredDataViewModel<CommentViewModel>
            {
                Data = comments,
                Count = commentsCount,
            };
        }
    }
}