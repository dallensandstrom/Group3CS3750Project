using GroupThreeTrailerParkProject.Data;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        if (context.Reservations.Any())
            return;

        context.Reservations.AddRange(
            new Reservation
            {
                AccountID = 1,
                CustomerName = "John Smith",
                SiteNumber = 12,
                CheckInDate = DateTime.Today.AddDays(5),
                CheckOutDate = DateTime.Today.AddDays(10),
                NumAdults = 2,
                Pets = 1,
                BaseCost = 200,
                TotalCost = 200,
                Status = "Confirmed",
                DateCreated = DateTime.Now,
                ExtraNotes = "Late arrival"
            },
            new Reservation
            {
                AccountID = 2,
                CustomerName = "Mary Johnson",
                SiteNumber = 7,
                CheckInDate = DateTime.Today.AddDays(3),
                CheckOutDate = DateTime.Today.AddDays(6),
                NumAdults = 4,
                Pets = 0,
                BaseCost = 150,
                TotalCost = 150,
                Status = "Confirmed",
                DateCreated = DateTime.Now,
                ExtraNotes = ""
            }
        );

        context.SaveChanges();
    }
}