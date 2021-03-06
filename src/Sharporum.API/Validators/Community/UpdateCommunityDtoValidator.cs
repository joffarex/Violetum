﻿using FluentValidation;
using Sharporum.Core.Dtos.Community;

namespace Sharporum.API.Validators.Community
{
    public class UpdateCommunityDtoValidator : AbstractValidator<UpdateCommunityDto>
    {
        public UpdateCommunityDtoValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}