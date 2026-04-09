using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GroupThreeTrailerParkProject.Controllers
{
    [Authorize(Roles = "Guest")]
    public class CustomerReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserAccount> _userManager;

        public CustomerReservationsController(
            ApplicationDbContext context,
            UserManager<UserAccount> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
    DateTime? startDate,
    DateTime? endDate,
    string? status,
    string? petFilter)
        {
            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null)
            {
                return Forbid();
            }

            var reservations = _context.Reservations
                .Include(r => r.Site)
                .Where(r => r.AccountID == accountId.Value)
                .AsQueryable();

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

            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null)
            {
                return Forbid();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Site)
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult Create()
        {
            PopulateSitesDropDownList();

            return View(new Reservation
            {
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var accountId = await GetCurrentGuestAccountIdAsync();

            if (currentUser == null || accountId == null)
            {
                return Forbid();
            }

            reservation.AccountID = accountId.Value;
            reservation.CustomerName = $"{currentUser.FirstName} {currentUser.LastName}".Trim();
            reservation.Status = string.IsNullOrWhiteSpace(reservation.Status)
                ? "Pending"
                : reservation.Status;

            if (!IsSiteAvailable(reservation.SiteId, reservation.CheckInDate, reservation.CheckOutDate))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
            }

            if (reservation.CheckOutDate <= reservation.CheckInDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
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

                return RedirectToAction(nameof(Review), new { id = reservation.ReservationID });
            }

            PopulateSitesDropDownList(reservation.SiteId);
            return View(reservation);
        }

        public async Task<IActionResult> Review(int? id)
        {
            if (id == null) return NotFound();
            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null) return Forbid();

            var reservation = await _context.Reservations
                .Include(r => r.Site)
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

            if (reservation == null) return NotFound();
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null) return Forbid();

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

            if (reservation == null) return NotFound();

            // Check if a payment already exists for this reservation
            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.ReservationId == id);

            if (existingPayment != null)
            {
                // Update existing payment with new amount
                existingPayment.Amount = reservation.TotalCost ?? 0;
                existingPayment.PaymentDate = DateTime.Now;
                _context.Payments.Update(existingPayment);
            }
            else
            {
                // Create new payment record
                var payment = new Payment
                {
                    ReservationId = reservation.ReservationID,
                    Amount = reservation.TotalCost ?? 0,
                    PaymentType = "Online",
                    PaymentDate = DateTime.Now,
                    Status = "Completed"
                };
                _context.Payments.Add(payment);
            }

            // Ensure reservation is marked as Confirmed
            reservation.Status = "Confirmed";
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation confirmed successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null)
            {
                return Forbid();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

            if (reservation == null)
            {
                return NotFound();
            }

            reservation.ExtraNotes = ExtractManualNotes(reservation.ExtraNotes);

            PopulateSitesDropDownList(reservation.SiteId);
            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.ReservationID)
            {
                return NotFound();
            }

            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null)
            {
                return Forbid();
            }

            var existingReservation = await _context.Reservations
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

            if (existingReservation == null)
            {
                return NotFound();
            }

            // Check if any actual changes were made
            bool hasChanges = existingReservation.CheckInDate != reservation.CheckInDate ||
                             existingReservation.CheckOutDate != reservation.CheckOutDate ||
                             existingReservation.NumAdults != reservation.NumAdults ||
                             existingReservation.Pets != reservation.Pets ||
                             existingReservation.SiteId != reservation.SiteId;

            reservation.AccountID = existingReservation.AccountID;
            reservation.CustomerName = existingReservation.CustomerName;
            reservation.Status = hasChanges ? "Pending" : existingReservation.Status;
            reservation.DateCreated = existingReservation.DateCreated;

            if (!IsSiteAvailable(
                reservation.SiteId,
                reservation.CheckInDate,
                reservation.CheckOutDate,
                reservation.ReservationID))
            {
                ModelState.AddModelError("", "That site is already reserved for those dates.");
            }

            if (reservation.CheckOutDate <= reservation.CheckInDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
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

                // Only redirect to Review if changes were made, otherwise go back to Index
                if (hasChanges)
                {
                    return RedirectToAction(nameof(Review), new { id = reservation.ReservationID });
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
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

            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null)
            {
                return Forbid();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Site)
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

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
            var accountId = await GetCurrentGuestAccountIdAsync();
            if (accountId == null)
            {
                return Forbid();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.ReservationID == id &&
                    r.AccountID == accountId.Value);

            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Canceled";
            reservation.BaseCost = 0;
            reservation.TotalCost = 0;
            _context.Update(reservation);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetReservationPrice(
            int siteId,
            DateTime checkInDate,
            DateTime checkOutDate,
            int pets)
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

        private async Task<int?> GetCurrentGuestAccountIdAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return null;
            }

            var guestProfile = await _context.GuestProfiles
                .FirstOrDefaultAsync(g => g.UserAccountID == currentUser.Id);

            return guestProfile?.Id;
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

            ViewBag.SiteId = new SelectList(sitesQuery, "SiteId", "SiteId", selectedSite);
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
    }
}