using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace jcamacho_journal.Models
{
    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? UpdatedDate { get; set; } = DateTimeOffset.Now;
        public string Title { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string MediaURL { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string Slug { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        // public string Slug { get; internal set; }

    }
}