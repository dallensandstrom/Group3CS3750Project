using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GroupThreeTrailerParkProject.Models
{
    public class UserAccount : IdentityUser //Uses default IdentityUser class for password, email, and phone -Dallen
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsEnabled { get; set; } = true;
    }
}
