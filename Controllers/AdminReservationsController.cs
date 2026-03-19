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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Reservations.ToListAsync());
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationID,AccountID,SiteNumber,CheckInDate,CheckOutDate,NumAdults,Pets,BaseCost,TotalCost,Status,DateCreated,ExtraNotes,CustomerName")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            return View(reservation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.ReservationID)
                return NotFound();

            if (!IsSiteAvailable(reservation.SiteNumber, reservation.CheckInDate, reservation.CheckOutDate, reservation.ReservationID))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
                return View(reservation);
            }

            if (ModelState.IsValid)
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

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
                r.SiteNumber == siteNumber &&
                r.ReservationID != reservationId &&
                r.Status != "Cancelled" &&
                checkIn < r.CheckOutDate &&
                checkOut > r.CheckInDate
            );
        }
    }
}
