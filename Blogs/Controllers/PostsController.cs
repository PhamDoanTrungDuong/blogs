using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blogs.Models;
using PagedList.Core;
using Blogs.Helpers;

namespace Blogs.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogsDBContext _context;

        public PostsController(BlogsDBContext context)
        {
            _context = context;
        }

        // GET: Posts
        [Route("{Alias}", Name = "ListTin")]
        public IActionResult List(string Alias, int? page)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Home", "Index");
            var danhmuc = _context.Categories.FirstOrDefault(x => x.Alias == Alias);
            if (danhmuc == null) return RedirectToAction("Home", "Index");

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
           // var pageSize = 5;

            List<Post> lsPosts = new List<Post>();

            if (!string.IsNullOrEmpty(Alias))
            {
                lsPosts = _context.Posts
                   .Include(x => x.Cat)
                   .Where(x => x.CatId == danhmuc.CatId)
                   .AsNoTracking()
                   .OrderByDescending(x => x.CatId)
                   .ToList();
            }
            else
            {
                lsPosts = _context.Posts
                  .Include(x => x.Cat)
                  .Where(x => x.CatId == danhmuc.CatId)
                  .AsNoTracking()
                  .OrderByDescending(x => x.CatId)
                  .ToList();
            }


            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            ViewBag.DanhMuc = danhmuc;
            return View(models);
        }

        // GET: Posts/Details/5
        [Route("{Alias}.html", Name = "PostsDetails")]
        public async Task<IActionResult> Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.Alias == Alias);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
