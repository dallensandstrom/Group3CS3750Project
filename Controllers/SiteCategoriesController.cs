using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index(
        int? siteCategoryId,
        int? minVehicleSize,
        DateTime? checkInDate,
        DateTime? checkOutDate)
        {
        var sitesQuery = _context.Site
        .Include(s => s.SiteCategory)
        .AsQueryable();

        // Guests should only see sites that are visible to clients
        if (User.IsInRole("Guest"))
        {
        sitesQuery = sitesQuery.Where(s => s.VisibleToClient);
        }

        // Filter by category
        if (siteCategoryId.HasValue)
        {
        sitesQuery = sitesQuery.Where(s => s.SiteCategoryId == siteCategoryId.Value);
        }

        // Filter by minimum vehicle size
        if (minVehicleSize.HasValue)
        {
        sitesQuery = sitesQuery.Where(s => s.MaxVehicleSize >= minVehicleSize.Value);
        }

        // Filter by availability
        if (checkInDate.HasValue && checkOutDate.HasValue && checkOutDate > checkInDate)
        {
        var start = checkInDate.Value.Date;
        var end = checkOutDate.Value.Date;

        sitesQuery = sitesQuery.Where(site =>
            !_context.Reservations.Any(r =>
                r.SiteId == site.SiteId &&
                r.Status != "Canceled" &&
                start < r.CheckOutDate &&
                end > r.CheckInDate));
        }

        ViewBag.SiteCategoryId = new SelectList(
        await _context.SiteCategory.OrderBy(c => c.Name).ToListAsync(),
        "SiteCategoryId",
        "Name",
        siteCategoryId);

        ViewBag.MinVehicleSize = minVehicleSize;
        ViewBag.CheckInDate = checkInDate?.ToString("yyyy-MM-dd");
        ViewBag.CheckOutDate = checkOutDate?.ToString("yyyy-MM-dd");

        var sites = await sitesQuery
        .OrderBy(s => s.SiteId)
        .ToListAsync();

        return View(sites);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var site = await _context.Site
                .Include(s => s.SiteCategory!) //Dallen - added null forgiving ! to remove warning
                    .ThenInclude(c => c.PriceRanges)
                .FirstOrDefaultAsync(s => s.SiteId == id);

            if (site == null)
            {
                return NotFound();
            }

            ViewBag.Photos = await _context.SitePhotos
                .Where(p => p.SiteId == site.SiteId)
                .ToListAsync();

            return View(site);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Site site, string NewPhotoUrl)
        {
            if (ModelState.IsValid)
            {
                _context.Site.Add(site);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(NewPhotoUrl)) //Added photos input (Dallen)
                {
                    var newPhoto = new SitePhoto
                    {
                        SiteId = site.SiteId,
                        PhotoUrl = NewPhotoUrl
                    };

                    _context.SitePhotos.Add(newPhoto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name", site.SiteCategoryId);
            return View(site);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CategoryManage()
        {
            ViewBag.Categories = await _context.SiteCategory
                .Include(c => c.PriceRanges)
                .ToListAsync();

            return View(new SiteCategory
            {
                Name = string.Empty,
                PriceRanges = new List<PriceRange>
                {
                    new PriceRange()
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CategoryManage(SiteCategory category, string submitButton)
        {
            if (submitButton == "Delete")
            {
                if (category.SiteCategoryId != 0)
                {
                    var existingCategory = await _context.SiteCategory
                        .Include(c => c.PriceRanges)
                        .FirstOrDefaultAsync(c => c.SiteCategoryId == category.SiteCategoryId);

                    if (existingCategory != null)
                    {
                        if (existingCategory.PriceRanges.Any())
                        {
                            _context.PriceRanges.RemoveRange(existingCategory.PriceRanges);
                        }

                        _context.SiteCategory.Remove(existingCategory);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            if (submitButton == "Apply")
            {
                if (category.PriceRanges == null)
                {
                    category.PriceRanges = new List<PriceRange>();
                }

                category.PriceRanges = category.PriceRanges
                    .Where(pr => pr != null)
                    .Where(pr => pr.Price > 0 || pr.StartDate != default || pr.EndDate.HasValue)
                    .ToList();

                if (!category.PriceRanges.Any())
                {
                    ModelState.AddModelError("", "At least one price range is required.");
                }

                foreach (var priceRange in category.PriceRanges)
                {
                    if (priceRange.EndDate.HasValue && priceRange.EndDate.Value < priceRange.StartDate)
                    {
                        ModelState.AddModelError("", "End date cannot be earlier than start date.");
                        break;
                    }
                }

                if (ModelState.IsValid)
                {
                    if (category.SiteCategoryId == 0)
                    {
                        _context.SiteCategory.Add(category);
                    }
                    else
                    {
                        var existingCategory = await _context.SiteCategory
                            .Include(c => c.PriceRanges)
                            .FirstOrDefaultAsync(c => c.SiteCategoryId == category.SiteCategoryId);

                        if (existingCategory == null)
                        {
                            return NotFound();
                        }

                        existingCategory.Name = category.Name;

                        if (existingCategory.PriceRanges.Any())
                        {
                            _context.PriceRanges.RemoveRange(existingCategory.PriceRanges);
                        }

                        existingCategory.PriceRanges = category.PriceRanges
                            .Select(pr => new PriceRange
                            {
                                SiteCategoryId = existingCategory.SiteCategoryId,
                                Price = pr.Price,
                                StartDate = pr.StartDate,
                                EndDate = pr.EndDate
                            })
                            .ToList();
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Categories = await _context.SiteCategory
                .Include(c => c.PriceRanges)
                .ToListAsync();

            if (category.PriceRanges == null || !category.PriceRanges.Any())
            {
                category.PriceRanges = new List<PriceRange>
                {
                    new PriceRange()
                };
            }

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
            
            // Hides if not visible
            if (User.IsInRole("Guest") && !site.VisibleToClient)
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
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Edit(int id, Site site, string submitButton, string NewPhotoUrl)
{
    if (id != site.SiteId)
    {
        return NotFound();
    }

    var existingSite = await _context.Site.FindAsync(site.SiteId);
    if (existingSite == null)
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

        _context.Site.Remove(existingSite);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    if (submitButton == "Apply")
    {
        existingSite.SiteCategoryId = site.SiteCategoryId;
        existingSite.MaxVehicleSize = site.MaxVehicleSize;
        existingSite.VisibleToClient = site.VisibleToClient;

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

    ViewBag.SiteCategoryId = new SelectList(_context.SiteCategory, "SiteCategoryId", "Name", site.SiteCategoryId);

    ViewBag.Photos = await _context.SitePhotos
        .Where(p => p.SiteId == site.SiteId)
        .ToListAsync();

    return View(site);
}
    }
}