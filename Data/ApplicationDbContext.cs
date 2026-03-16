using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GroupThreeTrailerParkProject.Models;

namespace GroupThreeTrailerParkProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<GroupThreeTrailerParkProject.Models.SiteModel> SiteModel { get; set; } = default!;
    }
}
