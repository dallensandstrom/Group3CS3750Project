namespace GroupThreeTrailerParkProject.Models
{
    // Simplified version of UserAccount for displaying user details in the view
    public class UserAccountViewModel
    {
        public string Id { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsEnabled { get; set; }
        public string Roles { get; set; } = "";
    }
}
