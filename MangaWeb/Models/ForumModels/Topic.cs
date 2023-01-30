﻿namespace MangaWeb.Models.ForumModels
{
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImgSrc { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<Post> Posts { get; set; }
    }
}
