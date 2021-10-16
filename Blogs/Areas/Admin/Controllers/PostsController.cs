using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blogs.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Http;
using Blogs.Helpers;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Blogs.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class PostsController : Controller
    {
        private readonly BlogsDBContext _context;

        public PostsController(BlogsDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
        public IActionResult Index(int? page, int CatID = 0)
        {
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            //var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
            //if (account == null) return NotFound();

            List<Post> lsPosts = new List<Post>();

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 1;//Utilities.PAGE_SIZE;

            if (CatID != 0)
            {
                lsPosts = _context.Posts
                   .AsNoTracking().Where(x => x.CatId == CatID)
                   .Include(p => p.Cat)
                   .OrderByDescending(x => x.PostId)
                   .ToList();
            }
            else
            {
                lsPosts = _context.Posts
                   .AsNoTracking()
                   .Include(p => p.Cat)
                   .OrderByDescending(x => x.CatId)
                   .ToList();
            }


            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentCat = CatID;
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(models);
        }

        public IActionResult Filtter(int catID = 0)
        {
            var url = $"/Admin/Posts/Index?catID={catID}";

            if(catID == 0)
            {
                url = $"/Admin/Posts/Index";
            }
            else
            {
                url = $"/Admin/Posts/Index?catID={catID}";
            }
            return Json(new { status = "success", Redirecturl = url });
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreateDate,Author,AccountId,Tags,CatId,IsHost,IsNewfeed")] Post post, Microsoft.AspNetCore.Http.IFormFile fthumb)
        {
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            //var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
            //if (account == null) return NotFound();

            if (ModelState.IsValid)
            {
               // post.AccountId = account.AccountId;
               // post.Author = account.FullName;
                if (post.CatId == null) post.CatId = 1;
                post.CreateDate = DateTime.Now;
                post.Alias = Utilities.ToUrlFriendly(post.Title);
                if (fthumb != null)
                {
                    string extension = Path.GetExtension(fthumb.FileName);
                    string newName = "icon_" + Utilities.ToUrlFriendly(post.Title) + "preview" + extension;
                    post.Thumb = await Utilities.UploadFile(fthumb, @"posts\", newName.ToLower());
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreateDate,Author,AccountId,Tags,CatId,IsHost,IsNewfeed")] Post post, Microsoft.AspNetCore.Http.IFormFile fthumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
           // var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
           // var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
            //if (account == null) return NotFound();

            //if (account.RoleId != 1)
            //{
            //    if (post.AccountId != account.AccountId) return RedirectToAction(nameof(Index));
            //}


            if (ModelState.IsValid)
            {
                try
                {
                    if (fthumb != null)
                    {
                        string extension = Path.GetExtension(fthumb.FileName);
                        string newName = "icon_" + Utilities.ToUrlFriendly(post.Title) + "preview" + extension;
                        post.Thumb = await Utilities.UploadFile(fthumb, @"posts\", newName.ToLower());
                    }
                    post.Alias = Utilities.ToUrlFriendly(post.Title);
                    //post.AccountId = account.AccountId;
                    //post.Author = account.FullName;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
