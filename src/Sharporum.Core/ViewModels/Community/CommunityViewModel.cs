﻿using Sharporum.Core.ViewModels.User;

namespace Sharporum.Core.ViewModels.Community
{
    public class CommunityViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public UserBaseViewModel Author { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}