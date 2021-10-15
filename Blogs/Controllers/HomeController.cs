using Blogs.Models;
using Blogs.ModelView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blogs.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogsDBContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, BlogsDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //var tin = _context.Posts.Find(1);
            //for(int i=1; i<=20; i++)
            //{
            //    Post post = new Post();
            //    post = tin;
            //    post.CatId = tin.CatId;
            //    post.Title = tin.Title;
            //    post.Scontents = tin.Scontents;
            //    post.Contents = tin.Contents;
            //    post.Thumb = tin.Thumb;
            //    post.Published = tin.Published;
            //    post.Alias = tin.Alias;
            //    post.CreateDate = tin.CreateDate;
            //    post.Author = tin.Author;
            //    post.AccountId = tin.AccountId;
            //    post.Tags = tin.Tags;
            //    post.IsHost = tin.IsHost;
            //    post.IsNewfeed = tin.IsNewfeed;
            //    post.CatId = tin.CatId;

            //    _context.Add(post);
            //    _context.SaveChanges();
            //}

            HomeVM model = new HomeVM();

            var ls = _context.Posts.Include(x => x.Cat).AsNoTracking().ToList();

            model.LatestPost = ls;
            model.Populars = ls;
            model.Recents = ls;
            model.Trendings = ls;
            model.Inspiration = ls;
            model.Featured = ls.FirstOrDefault();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
