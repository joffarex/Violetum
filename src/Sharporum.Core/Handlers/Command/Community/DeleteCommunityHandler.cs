﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Community;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Community
{
    public class DeleteCommunityHandler : IRequestHandler<DeleteCommunityCommand>
    {
        private readonly IBlobService _blobService;
        private readonly ICommunityService _communityService;

        public DeleteCommunityHandler(ICommunityService communityService, IBlobService blobService)
        {
            _communityService = communityService;
            _blobService = blobService;
        }

        public async Task<Unit> Handle(DeleteCommunityCommand request, CancellationToken cancellationToken)
        {
            if (!request.Community.Image.Equals($"{nameof(Community)}/no-image.jpg"))
            {
                await _blobService.DeleteBlobAsync(request.Community.Image);
            }

            await _communityService.DeleteCommunityAsync(request.Community);
            return Unit.Value;
        }
    }
}