using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blogs.Models;
using Blogs.Helpers;
using PagedList.Core;
using System.IO;

namespace Blogs.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly BlogsDBContext _context;
        
        public CategoriesController(BlogsDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 1;//Utilities.PAGE_SIZE;
            var lsCategories = _context.Categories
                .OrderByDescending(x => x.CatId);
            PagedList<Category> models = new PagedList<Category>(lsCategories, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parents,Levels,Icon,Cover,Description")] Category category,Microsoft.AspNetCore.Http.IFormFile fthumb, Microsoft.AspNetCore.Http.IFormFile fcover, Microsoft.AspNetCore.Http.IFormFile ficon)
        {
            if (ModelState.IsValid)
            {
                category.Alias = Utilities.ToUrlFriendly(category.CatName);
                if(category.Parents == null)
                {
                    category.Levels = 1;
                }
                else
                {
                    category.Levels = category.Parents == 0 ? 1 : 2;
                }
                if(fthumb != null)
                {
                    string extension = Path.GetExtension(fthumb.FileName);
                    string newName = Utilities.ToUrlFriendly(category.CatName) + "preview" + extension;
                    category.Thumb = await Utilities.UploadFile(fthumb, @"categories\", newName.ToLower());
                }
                if (fcover != null)
                {
                    string extension = Path.GetExtension(fcover.FileName);
                    string newName =  "cover_" + Utilities.ToUrlFriendly(category.CatName) + extension;
                    category.Thumb = await Utilities.UploadFile(fcover, @"covers\", newName.ToLower());
                }
                if (ficon != null)
                {
                    string extension = Path.GetExtension(ficon.FileName);
                    string newName = "icon_" + Utilities.ToUrlFriendly(category.CatName) + "preview" + extension;
                    category.Thumb = await Utilities.UploadFile(ficon, @"icons\", newName.ToLower());
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
            
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parents,Levels,Icon,Cover,Description")] Category category, Microsoft.AspNetCore.Http.IFormFile fthumb, Microsoft.AspNetCore.Http.IFormFile fcover, Microsoft.AspNetCore.Http.IFormFile ficon)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.Alias = Utilities.ToUrlFriendly(category.CatName);
                    if (category.Parents == null)
                    {
                        category.Levels = 1;
                    }
                    else
                    {
                        category.Levels = category.Parents == 0 ? 1 : 2;
                    }
                    if (fthumb != null)
                    {
                        string extension = Path.GetExtension(fthumb.FileName);
                        string newName = Utilities.ToUrlFriendly(category.CatName) + "preview" + extension;
                        category.Thumb = await Utilities.UploadFile(fthumb, @"categories\", newName.ToLower());
                    }
                    if (fcover != null)
                    {
                        string extension = Path.GetExtension(fcover.FileName);
                        string newName = "cover_" + Utilities.ToUrlFriendly(category.CatName) + extension;
                        category.Thumb = await Utilities.UploadFile(fcover, @"covers\", newName.ToLower());
                    }
                    if (ficon != null)
                    {
                        string extension = Path.GetExtension(ficon.FileName);
                        string newName = "icon_" + Utilities.ToUrlFriendly(category.CatName) + "preview" + extension;
                        category.Thumb = await Utilities.UploadFile(ficon, @"icons\", newName.ToLower());
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
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
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
