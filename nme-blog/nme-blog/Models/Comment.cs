using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nme_blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }

        // Author below vs AuthorId above
        public virtual ApplicationUser Author { get; set; } // Property Author here is in .Net User table and it is the reference to our property AuthorId in this Comment class as in: public string AuthorId { get; set; }
        // Post below vs PostId above
        public virtual BlogPost Post { get; set; }
    }
}