using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());


        if (!context.SiteCategory.Any())
        {
            context.SiteCategory.AddRange(
                new SiteCategory
                {
                    Name = "Standard RV Site",
                    Price = 40,
                    PricePerWeek = 250,
                    PricePerMonth = 800
                }
            );
        }

        context.SaveChanges();

        if (!context.Site.Any())
        {
            var category = context.SiteCategory.First();

            context.Site.AddRange(
                new Site
                {
                    SiteCategoryId = category.SiteCategoryId,
                    MaxVehicleSize = 40,
                    VisibleToClient = true,
                    DefaultPrice = 40
                }
            );
        }

        context.SaveChanges();

        if (!context.SitePhotos.Any())
        {
            var site = context.Site.First();

            context.SitePhotos.AddRange(
                new SitePhoto
                {
                    SiteId = site.SiteId,
                    PhotoUrl = "https://picsum.photos/400/250"
                }
            );
        }

        context.SaveChanges();

        if (!context.Reservations.Any())
        {
            context.Reservations.AddRange(
                new Reservation
                {
                    AccountID = 1,
                    CustomerName = "John Smith",
                    SiteId = 1,
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
                    SiteId = 1,
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
        }
        context.SaveChanges();

    }
    
}