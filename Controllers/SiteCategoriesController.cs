using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GroupThreeTrailerParkProject.Controllers
{
    [Authorize(Roles = "Guest,Employee,Admin")]
    public class SiteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SiteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sites = await _context.Site.ToListAsync();
            return View(sites);
        }

        public IActionResult Create()
        {
            ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Site site)
        {
            if (ModelState.IsValid)
            {
                _context.Site.Add(site);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name", site.SiteCategoryId);
            return View(site);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CategoryManage()
        {
            ViewBag.Categories = await _context.SiteCategory.ToListAsync();

            return View(new SiteCategory
            {
                Name = string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryManage(SiteCategory category, string submitButton)
        {
            if (submitButton == "Delete")
            {
                if (category.SiteCategoryId != 0)
                {
                    var existingCategory = await _context.SiteCategory.FindAsync(category.SiteCategoryId);
                    if (existingCategory != null)
                    {
                        _context.SiteCategory.Remove(existingCategory);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(CategoryManage));
            }

            if (submitButton == "Apply")
            {
                if (category.SiteCategoryId == 0)
                {
                    _context.SiteCategory.Add(category);
                }
                else
                {
                    var existingCategory = await _context.SiteCategory.FindAsync(category.SiteCategoryId);
                    if (existingCategory != null)
                    {
                        existingCategory.Name = category.Name;
                        existingCategory.Price = category.Price;
                        existingCategory.PricePerWeek = category.PricePerWeek;
                        existingCategory.PricePerMonth = category.PricePerMonth;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CategoryManage));
            }

            ViewBag.Categories = await _context.SiteCategory.ToListAsync();
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var site = await _context.Site.FindAsync(id);
            if (site == null)
            {
                return NotFound();
            }

            ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name", site.SiteCategoryId);

            ViewBag.Photos = await _context.SitePhotos
                .Where(p => p.SiteId == site.SiteId)
                .ToListAsync();

            return View(site);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Site site, string submitButton, string NewPhotoUrl)
        {
            if (id != site.SiteId)
            {
                return NotFound();
            }

            if (submitButton == "Delete")
            {
                var photos = await _context.SitePhotos
                    .Where(p => p.SiteId == site.SiteId)
                    .ToListAsync();

                if (photos.Any())
                {
                    _context.SitePhotos.RemoveRange(photos);
                }

                var existingSite = await _context.Site.FindAsync(site.SiteId);
                if (existingSite != null)
                {
                    _context.Site.Remove(existingSite);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (submitButton == "Apply")
            {
                if (ModelState.IsValid)
                {
                    var existingSite = await _context.Site.FindAsync(site.SiteId);
                    if (existingSite == null)
                    {
                        return NotFound();
                    }

                    existingSite.SiteCategoryId = site.SiteCategoryId;
                    existingSite.MaxVehicleSize = site.MaxVehicleSize;
                    existingSite.VisibleToClient = site.VisibleToClient;
                    existingSite.DefaultPrice = site.DefaultPrice;

                    if (!string.IsNullOrWhiteSpace(NewPhotoUrl))
                    {
                        var newPhoto = new SitePhoto
                        {
                            SiteId = site.SiteId,
                            PhotoUrl = NewPhotoUrl
                        };

                        _context.SitePhotos.Add(newPhoto);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Edit), new { id = site.SiteId });
                }
            }

            ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name", site.SiteCategoryId);

            ViewBag.Photos = await _context.SitePhotos
                .Where(p => p.SiteId == site.SiteId)
                .ToListAsync();

            return View(site);
        }
    }
}