using Blogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogs.Areas.Admin.Controllers
{
    public class SearchController : Controller
    {
        private readonly BlogsDBContext _context;

        public SearchController(BlogsDBContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult Find(string keyword)
        {
            if(keyword != null && keyword.Trim().Length > 3)
            {
                var ls = _context.Posts
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Where(x => x.Title.Contains(keyword) || x.Contents.Contains(keyword))
                    .OrderByDescending(x => x.CreateDate)
                    .ToList();
                return PartialView("ListBV", ls);
            }
            else
            {
                return PartialView("ListBV", null);
            }
        }
    }
}
