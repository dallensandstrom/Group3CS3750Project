using GroupThreeTrailerParkProject.Data;
using GroupThreeTrailerParkProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        var userManager = serviceProvider.GetRequiredService<UserManager<UserAccount>>();

        if (!context.Reservations.Any())
        {
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
        }

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

        await context.SaveChangesAsync();

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

        await context.SaveChangesAsync();

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

        await context.SaveChangesAsync();

        //Dallen Addition for seeding user accounts ChatGPT Suggestion
        await SeedUser(
            userManager,
            context,
            email: "admin@rvpark.com",
            password: "Password1!",
            firstName: "Admin",
            lastName: "User",
            phoneNumber: "5555550001",
            role: "Admin");

        await SeedUser(
            userManager,
            context,
            email: "employee@rvpark.com",
            password: "Password1!",
            firstName: "Employee",
            lastName: "User",
            phoneNumber: "5555550002",
            role: "Employee");

        await SeedUser(
            userManager,
            context,
            email: "guest@rvpark.com",
            password: "Password1!",
            firstName: "Guest",
            lastName: "User",
            phoneNumber: "5555550003",
            role: "Guest",
            dodAffiliation: DODAffiliation.Other,
            dodStatus: DODStatus.Retired);
    }

    //Dallen addition SeedUser function
    private static async Task SeedUser(
        UserManager<UserAccount> userManager,
        ApplicationDbContext context,
        string email,
        string password,
        string firstName,
        string lastName,
        string phoneNumber,
        string role,
        DODAffiliation? dodAffiliation = null,
        DODStatus? dodStatus = null)
    {
        //If the user already exists do nothing
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
            return;

        //Object for all the user account information
        user = new UserAccount
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            IsEnabled = true,
            EmailConfirmed = true
        };

        //Use user object and password to create the user
        var result = await userManager.CreateAsync(user, password);

        //Throw error if result fails
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create seed user");
        }

        //Add the role
        await userManager.AddToRoleAsync(user, role);

        //If the role is guest add the dodAffiliation and dodStatus
        if (role == "Guest" && dodAffiliation.HasValue && dodStatus.HasValue)
        {
            context.GuestProfiles.Add(new GuestProfile
            {
                UserAccountID = user.Id,
                DODAffiliation = dodAffiliation.Value,
                DODStatus = dodStatus.Value
            });

            await context.SaveChangesAsync();
        }
    }
}
    
