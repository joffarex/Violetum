﻿namespace Violetum.ApplicationCore.ViewModels
{
    public class PostViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public AuthorViewModel Author { get; set; }
        public PostCategoryViewModel Category { get; set; }
        public string CreatedAt { get; set; }
    }
}