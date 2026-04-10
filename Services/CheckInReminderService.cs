using GroupThreeTrailerParkProject.Data;
using Microsoft.EntityFrameworkCore;

namespace GroupThreeTrailerParkProject.Services
{
    public class CheckInReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CheckInReminderService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Check every 6 hours

        public CheckInReminderService(
            IServiceProvider serviceProvider,
            ILogger<CheckInReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Check-In Reminder Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendCheckInRemindersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending check-in reminders");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Check-In Reminder Service stopped");
        }

        private async Task SendCheckInRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            // Get today's date and the date 3 days from now
            var today = DateTime.Today;
            var reminderDate = today.AddDays(3);

            // Find all confirmed reservations that check in exactly 3 days from now
            var upcomingReservations = await context.Reservations
                .Where(r => r.Status == "Confirmed" &&
                           r.CheckInDate.Date == reminderDate.Date)
                .ToListAsync();

            _logger.LogInformation($"Found {upcomingReservations.Count} reservations with check-in on {reminderDate:yyyy-MM-dd}");

            foreach (var reservation in upcomingReservations)
            {
                try
                {
                    // Get the guest's email
                    var guestProfile = await context.GuestProfiles
                        .Include(g => g.UserAccount)
                        .FirstOrDefaultAsync(g => g.Id == reservation.AccountID);

                    if (guestProfile?.UserAccount?.Email != null)
                    {
                        await emailService.SendCheckInReminderEmailAsync(
                            guestProfile.UserAccount.Email,
                            reservation.CustomerName,
                            reservation.ReservationID,
                            reservation.CheckInDate,
                            reservation.SiteId,
                            reservation.ExtraNotes ?? ""
                        );

                        _logger.LogInformation($"Sent check-in reminder for reservation {reservation.ReservationID} to {guestProfile.UserAccount.Email}");
                    }
                    else
                    {
                        _logger.LogWarning($"Could not find email for reservation {reservation.ReservationID}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send check-in reminder for reservation {reservation.ReservationID}");
                }
            }
        }
    }
}
