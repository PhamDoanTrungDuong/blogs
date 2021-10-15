using Blogs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogs.ModelView
{
    public class HomeVM
    {
        public List<Post> Populars { get; set; }
        public List<Post> Inspiration { get; set; }
        public List<Post> Recents { get; set; }
        public List<Post> Trendings { get; set; }
        public List<Post> LatestPost { get; set; }
        public Post Featured { get; set; }
    }
}
