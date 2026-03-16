using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;

namespace GroupThreeTrailerParkProject.Controllers
{
    public class SiteCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SiteCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.SiteCategory.ToListAsync());
        }

        // GET: SiteCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteCategory = await _context.SiteCategory
                .FirstOrDefaultAsync(m => m.SiteCategoryId == id);
            if (siteCategory == null)
            {
                return NotFound();
            }

            return View(siteCategory);
        }

        // GET: SiteCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SiteCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SiteCategoryId,Name,Price,PricePerWeek,PricePerMonth")] SiteCategory siteCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(siteCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteCategory);
        }

        // GET: SiteCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteCategory = await _context.SiteCategory.FindAsync(id);
            if (siteCategory == null)
            {
                return NotFound();
            }
            return View(siteCategory);
        }

        // POST: SiteCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SiteCategoryId,Name,Price,PricePerWeek,PricePerMonth")] SiteCategory siteCategory)
        {
            if (id != siteCategory.SiteCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteCategoryExists(siteCategory.SiteCategoryId))
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
            return View(siteCategory);
        }

        // GET: SiteCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteCategory = await _context.SiteCategory
                .FirstOrDefaultAsync(m => m.SiteCategoryId == id);
            if (siteCategory == null)
            {
                return NotFound();
            }

            return View(siteCategory);
        }

        // POST: SiteCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var siteCategory = await _context.SiteCategory.FindAsync(id);
            if (siteCategory != null)
            {
                _context.SiteCategory.Remove(siteCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteCategoryExists(int id)
        {
            return _context.SiteCategory.Any(e => e.SiteCategoryId == id);
        }
    }
}
