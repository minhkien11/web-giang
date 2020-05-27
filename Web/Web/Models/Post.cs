using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string UnsignedTitle { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CoverImage { get; set; }
    }
}
