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
    public class PriceRangesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriceRangesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PriceRanges
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PriceRanges.Include(p => p.Site);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PriceRanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceRange = await _context.PriceRanges
                .Include(p => p.Site)
                .FirstOrDefaultAsync(m => m.PriceRangeID == id);
            if (priceRange == null)
            {
                return NotFound();
            }

            return View(priceRange);
        }

        // GET: PriceRanges/Create
        public IActionResult Create()
        {
            ViewData["SiteId"] = new SelectList(_context.Site, "SiteId", "SiteId");
            return View();
        }

        // POST: PriceRanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PriceRangeID,SiteId,StartDate,EndDate,Price")] PriceRange priceRange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceRange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ADD THIS to see what's failing
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            ViewData["SiteId"] = new SelectList(_context.Site, "SiteId", "SiteId", priceRange.SiteId);
            return View(priceRange);
        }

        // GET: PriceRanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceRange = await _context.PriceRanges.FindAsync(id);
            if (priceRange == null)
            {
                return NotFound();
            }
            ViewData["SiteId"] = new SelectList(_context.Site, "SiteId", "SiteId", priceRange.SiteId);
            return View(priceRange);
        }

        // POST: PriceRanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PriceRangeID,SiteId,StartDate,EndDate,Price")] PriceRange priceRange)
        {
            if (id != priceRange.PriceRangeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceRange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceRangeExists(priceRange.PriceRangeID))
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
            ViewData["SiteId"] = new SelectList(_context.Site, "SiteId", "SiteId", priceRange.SiteId);
            return View(priceRange);
        }

        // GET: PriceRanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceRange = await _context.PriceRanges
                .Include(p => p.Site)
                .FirstOrDefaultAsync(m => m.PriceRangeID == id);
            if (priceRange == null)
            {
                return NotFound();
            }

            return View(priceRange);
        }

        // POST: PriceRanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceRange = await _context.PriceRanges.FindAsync(id);
            if (priceRange != null)
            {
                _context.PriceRanges.Remove(priceRange);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceRangeExists(int id)
        {
            return _context.PriceRanges.Any(e => e.PriceRangeID == id);
        }
    }
}
