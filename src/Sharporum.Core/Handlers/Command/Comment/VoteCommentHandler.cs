﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Comment;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Comment
{
    public class VoteCommentHandler : IRequestHandler<VoteCommentCommand>
    {
        private readonly ICommentService _commentService;

        public VoteCommentHandler(ICommentService commentService)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        public async Task<Unit> Handle(VoteCommentCommand request, CancellationToken cancellationToken)
        {
            await _commentService.VoteCommentAsync(request.CommentId, request.UserId, request.CommentVoteDto);

            return Unit.Value;
        }
    }
}