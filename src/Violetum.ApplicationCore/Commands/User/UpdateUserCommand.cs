﻿using MediatR;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Responses;

namespace Violetum.ApplicationCore.Commands.User
{
    public class UpdateUserCommand : IRequest<UserResponse>
    {
        public UpdateUserCommand(string userId, UpdateUserDto updateUserDto)
        {
            UserId = userId;
            UpdateUserDto = updateUserDto;
        }

        public string UserId { get; set; }
        public UpdateUserDto UpdateUserDto { get; set; }
    }
}