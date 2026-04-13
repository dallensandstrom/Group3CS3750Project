using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupThreeTrailerParkProject.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class AdminReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
    string? searchString,
    DateTime? startDate,
    DateTime? endDate,
    string? status,
    string? petFilter)
        {
            var reservations = _context.Reservations
                .Include(r => r.Site)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                reservations = reservations.Where(r =>
                    r.CustomerName.Contains(searchString) ||
                    r.ReservationID.ToString() == searchString);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var start = startDate.Value.Date;
                var end = endDate.Value.Date;

                reservations = reservations.Where(r =>
                    r.CheckInDate.Date <= end &&
                    r.CheckOutDate.Date >= start);
            }
            else if (startDate.HasValue)
            {
                var start = startDate.Value.Date;

                reservations = reservations.Where(r =>
                    r.CheckOutDate.Date >= start);
            }
            else if (endDate.HasValue)
            {
                var end = endDate.Value.Date;

                reservations = reservations.Where(r =>
                    r.CheckInDate.Date <= end);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                reservations = reservations.Where(r => r.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(petFilter))
            {
                if (petFilter == "HasPets")
                {
                    reservations = reservations.Where(r => r.Pets >= 1);
                }
                else if (petFilter == "NoPets")
                {
                    reservations = reservations.Where(r => r.Pets == 0);
                }
            }

            ViewBag.SearchString = searchString;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;
            ViewBag.PetFilter = petFilter;

            ViewBag.StatusList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "All" },
                    new SelectListItem { Value = "Pending", Text = "Pending" },
                    new SelectListItem { Value = "Confirmed", Text = "Confirmed" },
                    new SelectListItem { Value = "Canceled", Text = "Canceled" }
                },
                "Value",
                "Text",
                status
            );

            ViewBag.PetFilterList = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "All" },
                    new SelectListItem { Value = "HasPets", Text = "Pets" },
                    new SelectListItem { Value = "NoPets", Text = "No Pets" }
                },
                "Value",
                "Text",
                petFilter
            );

            return View(await reservations
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync());
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

        [Authorize(Roles = "Employee,Admin")]
        public IActionResult Create()
        {
            PopulateSitesDropDownList();
            PopulateStatusDropDownList("Pending");

            return View(new Reservation
            {
                Status = "Pending",
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (!IsSiteAvailable(reservation.SiteId, reservation.CheckInDate, reservation.CheckOutDate))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
            }

            if (reservation.CheckOutDate <= reservation.CheckInDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
            }

            if (!IsValidStatus(reservation.Status))
            {
                ModelState.AddModelError("Status", "Status must be Pending, Confirmed, or Canceled.");
            }

            if (ModelState.IsValid)
            {
                reservation.DateCreated = DateTime.Now;

                reservation.BaseCost = await CalculateBaseCostAsync(
                    reservation.SiteId,
                    reservation.CheckInDate,
                    reservation.CheckOutDate);

                var siteFees = await CalculateSiteFeesAsync(
                    reservation.SiteId,
                    reservation.CheckInDate,
                    reservation.CheckOutDate);

                var reservationFees = await CalculateReservationFeesAsync(reservation);

                reservation.TotalCost = reservation.BaseCost + siteFees + reservationFees;

                var feeNames = await GetAppliedFeeNamesAsync(reservation);

                reservation.ExtraNotes = BuildExtraNotes(reservation.ExtraNotes, feeNames);

                _context.Add(reservation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateSitesDropDownList(reservation.SiteId);
            PopulateStatusDropDownList(reservation.Status);
            return View(reservation);
        }

        [Authorize(Roles = "Employee,Admin")]
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

            reservation.ExtraNotes = ExtractManualNotes(reservation.ExtraNotes);

            PopulateSitesDropDownList(reservation.SiteId);
            PopulateStatusDropDownList(reservation.Status);

            return View(reservation);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.ReservationID)
                return NotFound();

            var existingReservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReservationID == id);

            if (existingReservation == null)
                return NotFound();

            reservation.DateCreated = existingReservation.DateCreated;

            if (!IsSiteAvailable(reservation.SiteId, reservation.CheckInDate, reservation.CheckOutDate, reservation.ReservationID))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
            }

            if (reservation.CheckOutDate <= reservation.CheckInDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
            }

            if (!IsValidStatus(reservation.Status))
            {
                ModelState.AddModelError("Status", "Status must be Pending, Confirmed, or Canceled.");
            }

            if (ModelState.IsValid)
            {
                reservation.BaseCost = await CalculateBaseCostAsync(
                    reservation.SiteId,
                    reservation.CheckInDate,
                    reservation.CheckOutDate);

                var siteFees = await CalculateSiteFeesAsync(
                    reservation.SiteId,
                    reservation.CheckInDate,
                    reservation.CheckOutDate);

                var reservationFees = await CalculateReservationFeesAsync(reservation);

                reservation.TotalCost = reservation.BaseCost + siteFees + reservationFees;

                var feeNames = await GetAppliedFeeNamesAsync(reservation);
                reservation.ExtraNotes = BuildExtraNotes(reservation.ExtraNotes, feeNames);

                _context.Update(reservation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            PopulateSitesDropDownList(reservation.SiteId);
            PopulateStatusDropDownList(reservation.Status);
            return View(reservation);
        }

        [Authorize(Roles = "Employee, Admin")]
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
        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Canceled";
            _context.Update(reservation);

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
                r.Status != "Canceled" &&
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
        private async Task<decimal> GetNightlyRateForDateAsync(int siteId, DateTime date)
        {
            var site = await _context.Site
                .Include(s => s.SiteCategory)
                .FirstOrDefaultAsync(s => s.SiteId == siteId);

            if (site == null)
                throw new Exception("Selected site was not found.");

            var priceRange = await _context.PriceRanges
                .Where(p => p.SiteCategoryId == site.SiteCategoryId)
                .Where(p => p.StartDate.Date <= date.Date &&
                           (p.EndDate == null || p.EndDate.Value.Date >= date.Date))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefaultAsync();

            if (priceRange == null)
                throw new Exception("No price range found for the selected site and date.");

            return priceRange.Price;
        }
        private async Task<decimal> CalculateBaseCostAsync(int siteId, DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkOutDate <= checkInDate)
                throw new Exception("Check-out date must be after check-in date.");

            decimal total = 0m;

            for (var date = checkInDate.Date; date < checkOutDate.Date; date = date.AddDays(1))
            {
                total += await GetNightlyRateForDateAsync(siteId, date);
            }

            return total;
        }
        [HttpGet]
        public async Task<IActionResult> GetReservationPrice(int siteId, DateTime checkInDate, DateTime checkOutDate, int pets)
        {
            if (checkOutDate <= checkInDate)
            {
                return Json(new
                {
                    success = false,
                    message = "Check-out date must be after check-in date."
                });
            }

            try
            {
                var tempReservation = new Reservation
                {
                    SiteId = siteId,
                    CheckInDate = checkInDate,
                    CheckOutDate = checkOutDate,
                    Pets = pets
                };

                var baseCost = await CalculateBaseCostAsync(siteId, checkInDate, checkOutDate);
                var siteFees = await CalculateSiteFeesAsync(siteId, checkInDate, checkOutDate);
                var reservationFees = await CalculateReservationFeesAsync(tempReservation);
                var totalCost = baseCost + siteFees + reservationFees;

                return Json(new
                {
                    success = true,
                    baseCost,
                    totalCost,
                    siteFees,
                    reservationFees
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        private async Task<decimal> CalculateSiteFeesAsync(int siteId, DateTime checkInDate, DateTime checkOutDate)
        {
            var siteFees = await _context.SiteFees
                .Where(sf => sf.SiteId == siteId)
                .Include(sf => sf.Fee)
                .ToListAsync();

            decimal totalFees = 0m;

            foreach (var siteFee in siteFees)
            {
                if (siteFee.Fee == null)
                    continue;

                for (var date = checkInDate.Date; date < checkOutDate.Date; date = date.AddDays(1))
                {
                    if (siteFee.Fee.AppliesOnDate(date))
                    {
                        totalFees += siteFee.Fee.Amount;
                    }
                }
            }

            return totalFees;
        }
        private async Task<decimal> CalculateReservationFeesAsync(Reservation reservation)
        {
            decimal totalFees = 0m;

            if (reservation.Pets >= 1)
            {
                var petFee = await _context.Fees
                    .FirstOrDefaultAsync(f => f.Name == "Pet Fee" && f.AppliesTo == "Reservation");

                if (petFee != null)
                {
                    totalFees += petFee.Amount;
                }
            }

            return totalFees;
        }
        private async Task<List<string>> GetAppliedFeeNamesAsync(Reservation reservation)
        {
            var feeNames = new List<string>();

            var siteFees = await _context.SiteFees
                .Where(sf => sf.SiteId == reservation.SiteId)
                .Include(sf => sf.Fee)
                .ToListAsync();

            foreach (var siteFee in siteFees)
            {
                if (siteFee.Fee == null)
                    continue;

                for (var date = reservation.CheckInDate.Date; date < reservation.CheckOutDate.Date; date = date.AddDays(1))
                {
                    if (siteFee.Fee.AppliesOnDate(date))
                    {
                        feeNames.Add(siteFee.Fee.Name);
                        break;
                    }
                }
            }

            if (reservation.Pets >= 1)
            {
                var petFee = await _context.Fees
                    .FirstOrDefaultAsync(f => f.Name == "Pet Fee" && f.AppliesTo == "Reservation");

                if (petFee != null)
                {
                    feeNames.Add(petFee.Name);
                }
            }

            return feeNames.Distinct().ToList();
        }
        private string BuildExtraNotes(string? existingNotes, List<string> feeNames)
        {
            var feeText = feeNames.Any()
                ? $"Fees Applied: {string.Join(", ", feeNames)}"
                : "";

            if (!string.IsNullOrEmpty(existingNotes) && existingNotes.StartsWith("Fees Applied:"))
            {
                var index = existingNotes.IndexOf("\n");
                existingNotes = index >= 0 ? existingNotes.Substring(index + 1) : "";

                if (!string.IsNullOrEmpty(existingNotes) && existingNotes.StartsWith("Notes:"))
                {
                    existingNotes = existingNotes.Substring("Notes:".Length).TrimStart();
                }
            }

            if (!string.IsNullOrEmpty(feeText))
            {
                if (!string.IsNullOrWhiteSpace(existingNotes))
                {
                    return $"{feeText}\nNotes: {existingNotes}";
                }
                else
                {
                    return $"{feeText}\nNotes:";
                }
            }

            return existingNotes ?? "";
        }
        private string ExtractManualNotes(string? extraNotes)
        {
            if (string.IsNullOrWhiteSpace(extraNotes))
                return string.Empty;

            var text = extraNotes;

            if (text.StartsWith("Fees Applied:"))
            {
                var notesIndex = text.IndexOf("Notes:");
                if (notesIndex >= 0)
                {
                    return text.Substring(notesIndex + "Notes:".Length).Trim();
                }

                return string.Empty;
            }

            return text;
        }
        private static readonly List<string> AllowedStatuses = new()
{
            "Pending",
            "Confirmed",
            "Canceled"
};

        private bool IsValidStatus(string? status)
        {
            return !string.IsNullOrWhiteSpace(status) && AllowedStatuses.Contains(status);
        }

        private void PopulateStatusDropDownList(string? selectedStatus = null)
        {
            ViewBag.StatusList = new SelectList(AllowedStatuses, selectedStatus);
        }
    }
}
