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
    public class SiteModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SiteModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.SiteModel.ToListAsync());
        }

        // GET: SiteModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteModel = await _context.SiteModel
                .FirstOrDefaultAsync(m => m.SiteId == id);
            if (siteModel == null)
            {
                return NotFound();
            }

            return View(siteModel);
        }

        // GET: SiteModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SiteModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SiteId,SiteCategoryId,MaxVehicleSize,VisibleToClient,DefaultPrice")] SiteModel siteModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(siteModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteModel);
        }

        // GET: SiteModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteModel = await _context.SiteModel.FindAsync(id);
            if (siteModel == null)
            {
                return NotFound();
            }
            return View(siteModel);
        }

        // POST: SiteModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SiteId,SiteCategoryId,MaxVehicleSize,VisibleToClient,DefaultPrice")] SiteModel siteModel)
        {
            if (id != siteModel.SiteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteModelExists(siteModel.SiteId))
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
            return View(siteModel);
        }

        // GET: SiteModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteModel = await _context.SiteModel
                .FirstOrDefaultAsync(m => m.SiteId == id);
            if (siteModel == null)
            {
                return NotFound();
            }

            return View(siteModel);
        }

        // POST: SiteModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var siteModel = await _context.SiteModel.FindAsync(id);
            if (siteModel != null)
            {
                _context.SiteModel.Remove(siteModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteModelExists(int id)
        {
            return _context.SiteModel.Any(e => e.SiteId == id);
        }
    }
}
