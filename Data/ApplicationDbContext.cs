using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GroupThreeTrailerParkProject.Models;

namespace GroupThreeTrailerParkProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserAccount> //Added UserAccount -Dallen
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Site> Site { get; set; } = default!;

        public DbSet<SiteCategory> SiteCategory { get; set; } = default!;

        public DbSet<SitePhoto> SitePhotos { get; set; } = default!;
        public DbSet<GuestProfile> GuestProfiles { get; set; } = default!; //Added GuestProfiles setup -Dallen
    }
}
