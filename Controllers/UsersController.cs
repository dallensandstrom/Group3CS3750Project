using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupThreeTrailerParkProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {

        // Manages user accounts and their roles (end line 21)
        private readonly UserManager<UserAccount> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<UserAccount> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Displays a list of users with their roles (end line 58)
        public async Task<IActionResult> Index()
        {
            // Get all users
            var allUsers = _userManager.Users.ToList();

            // Filter to only Employee and Admin users
            var employeeUsers = new List<UserAccount>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Employee") || roles.Contains("Admin"))
                {
                    employeeUsers.Add(user);
                }
            }

            // Create a view model with user details and their roles
            var userViewModels = new List<UserAccountViewModel>();

            foreach (var user in employeeUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserAccountViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsEnabled = user.IsEnabled,
                    Roles = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        // Toggles the enabled/disabled status of a user account w/ userID (end line 88)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User ID is required." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Toggle the IsEnabled status
            user.IsEnabled = !user.IsEnabled;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Json(new { success = true, isEnabled = user.IsEnabled });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update user status." });
            }
        }
    }
}
