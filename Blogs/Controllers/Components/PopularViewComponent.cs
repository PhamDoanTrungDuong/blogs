using Blogs.Enums;
using Blogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogs.Controllers.Components
{
    public class PopularViewComponent : ViewComponent
    {
        private readonly BlogsDBContext _context;
        private IMemoryCache _memoryCache;
        public PopularViewComponent(BlogsDBContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var _tinseo = _memoryCache.GetOrCreate(CacheKeys.Popular, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsCategories();
            });
            return View(_tinseo);
        }

        public List<Post> GetlsCategories()
        {
            List<Post> lstins = new List<Post>();
            lstins = _context.Posts
                .Where(x => x.Published == true)
                .Take(6)
                .ToList();
            return lstins;
        }
    }
}
