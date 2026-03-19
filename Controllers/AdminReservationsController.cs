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
    public class AdminReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var reservations = _context.Reservations
                .Include(r => r.Site)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                reservations = reservations.Where(r =>
                    r.CustomerName.Contains(searchString) ||
                    r.ReservationID.ToString() == searchString);
            }

            return View(await reservations.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult Create()
        {
            PopulateSitesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            reservation.DateCreated = DateTime.Now;

            if (!IsSiteAvailable(reservation.SiteId, reservation.CheckInDate, reservation.CheckOutDate))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSitesDropDownList(reservation.SiteId);

            return View(reservation);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            PopulateSitesDropDownList(reservation.SiteId);
            return View(reservation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.ReservationID)
                return NotFound();

            if (!IsSiteAvailable(reservation.SiteId, reservation.CheckInDate, reservation.CheckOutDate, reservation.ReservationID))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
            }

            var existingReservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ReservationID == id);

            reservation.DateCreated = existingReservation.DateCreated;

            if (ModelState.IsValid)
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSitesDropDownList(reservation.SiteId);
            return View(reservation);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.ReservationID == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationID == id);
        }
        private bool IsSiteAvailable(int siteNumber, DateTime checkIn, DateTime checkOut, int? reservationId = null)
        {
            return !_context.Reservations.Any(r =>
                r.SiteId == siteNumber &&
                r.ReservationID != reservationId &&
                r.Status != "Cancelled" &&
                checkIn < r.CheckOutDate &&
                checkOut > r.CheckInDate
            );
        }
        private void PopulateSitesDropDownList(object? selectedSite = null)
        {
            var sitesQuery = _context.Site
                .OrderBy(s => s.SiteId)
                .ToList();

            ViewBag.SiteID = new SelectList(sitesQuery, "SiteId", "SiteId", selectedSite);
        }
    }
}
