using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GroupThreeTrailerParkProject.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool IsGuest { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "DOD Affiliation")]
            public DODAffiliation? DODAffiliation { get; set; }

            [Display(Name = "DOD Status")]
            public DODStatus? DODStatus { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IsGuest = await _userManager.IsInRoleAsync(user, "Guest");

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            // Load guest-specific info if applicable
            if (IsGuest)
            {
                var guestProfile = await _context.GuestProfiles
                    .FirstOrDefaultAsync(g => g.UserAccountID == user.Id);

                if (guestProfile != null)
                {
                    Input.DODAffiliation = guestProfile.DODAffiliation;
                    Input.DODStatus = guestProfile.DODStatus;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Input.Email = user.Email;
            ModelState.Remove("Input.Email");

            IsGuest = await _userManager.IsInRoleAsync(user, "Guest");

            if (!ModelState.IsValid)
            {
                if (IsGuest)
                {
                    var guestProfile = await _context.GuestProfiles
                        .FirstOrDefaultAsync(g => g.UserAccountID == user.Id);
                    if (guestProfile != null)
                    {
                        Input.DODAffiliation = guestProfile.DODAffiliation;
                        Input.DODStatus = guestProfile.DODStatus;
                    }
                }
                return Page();
            }

            // Update user properties
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.PhoneNumber = Input.PhoneNumber;

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Update guest-specific info if applicable
            if (IsGuest && Input.DODAffiliation.HasValue && Input.DODStatus.HasValue)
            {
                var guestProfile = await _context.GuestProfiles
                    .FirstOrDefaultAsync(g => g.UserAccountID == user.Id);

                if (guestProfile != null)
                {
                    guestProfile.DODAffiliation = Input.DODAffiliation.Value;
                    guestProfile.DODStatus = Input.DODStatus.Value;
                    await _context.SaveChangesAsync();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Your profile has been updated successfully.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync(string confirmText)
        {
            if (confirmText != "DELETE")
            {
                TempData["ErrorMessage"] = "Account deletion cancelled. Confirmation text did not match.";
                return RedirectToPage();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var isGuest = await _userManager.IsInRoleAsync(user, "Guest");

            // If guest, cancel all pending/confirmed reservations and delete guest profile
            if (isGuest)
            {
                var guestProfile = await _context.GuestProfiles
                    .FirstOrDefaultAsync(g => g.UserAccountID == user.Id);

                if (guestProfile != null)
                {
                    // Cancel all active reservations
                    var activeReservations = await _context.Reservations
                        .Where(r => r.AccountID == guestProfile.Id && 
                               (r.Status == "Pending" || r.Status == "Confirmed"))
                        .ToListAsync();

                    foreach (var reservation in activeReservations)
                    {
                        reservation.Status = "Canceled";
                        reservation.BaseCost = 0;
                        reservation.TotalCost = 0;
                    }

                    // Delete guest profile
                    _context.GuestProfiles.Remove(guestProfile);
                    await _context.SaveChangesAsync();
                }
            }

            // Delete the user account
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting your account.";
                return RedirectToPage();
            }

            await _signInManager.SignOutAsync();

            return Redirect("~/");
        }
    }
}
