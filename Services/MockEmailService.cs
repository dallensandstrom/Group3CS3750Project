using System.Text;

namespace GroupThreeTrailerParkProject.Services
{
    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendPaymentConfirmationEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime checkInDate, DateTime checkOutDate, int siteId, 
            decimal totalCost, string extraNotes)
        {
            var emailContent = new StringBuilder();
            emailContent.AppendLine("\n╔════════════════════════════════════════════════════════════════╗");
            emailContent.AppendLine("║          📧 PAYMENT CONFIRMATION EMAIL (MOCK)                 ║");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            emailContent.AppendLine($"  To:      {toEmail}");
            emailContent.AppendLine($"  Subject: Reservation Confirmed - Confirmation #{reservationId}");
            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine($"  Dear {customerName},");
            emailContent.AppendLine("  Thank you for your reservation! Your payment has been processed.");
            emailContent.AppendLine();
            emailContent.AppendLine("  RESERVATION DETAILS:");
            emailContent.AppendLine($"    • Confirmation #: {reservationId}");
            emailContent.AppendLine($"    • Site Number:    {siteId}");
            emailContent.AppendLine($"    • Check-In:       {checkInDate:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Check-Out:      {checkOutDate:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Total Cost:     ${totalCost:F2}");

            if (!string.IsNullOrWhiteSpace(extraNotes))
            {
                emailContent.AppendLine();
                emailContent.AppendLine($"  NOTES: {extraNotes}");
            }

            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine("  ✅ This email was NOT actually sent (using MockEmailService)");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝\n");

            _logger.LogInformation(emailContent.ToString());
            Console.WriteLine(emailContent.ToString());

            return Task.CompletedTask;
        }

        public Task SendReservationModifiedEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime oldCheckIn, DateTime newCheckIn, DateTime oldCheckOut, 
            DateTime newCheckOut, int oldSiteId, int newSiteId, int oldNumAdults, int newNumAdults,
            int oldPets, int newPets, decimal oldCost, decimal newCost)
        {
            var emailContent = new StringBuilder();
            emailContent.AppendLine("\n╔════════════════════════════════════════════════════════════════╗");
            emailContent.AppendLine("║          📝 RESERVATION MODIFIED EMAIL (MOCK)                 ║");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            emailContent.AppendLine($"  To:      {toEmail}");
            emailContent.AppendLine($"  Subject: Reservation Updated - Confirmation #{reservationId}");
            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine($"  Dear {customerName},");
            emailContent.AppendLine("  Your reservation has been updated.");
            emailContent.AppendLine();
            emailContent.AppendLine("  WHAT CHANGED:");

            bool hasChanges = false;

            if (oldSiteId != newSiteId)
            {
                emailContent.AppendLine($"    • Site Number:    {newSiteId} (was {oldSiteId})");
                hasChanges = true;
            }

            if (oldCheckIn != newCheckIn)
            {
                emailContent.AppendLine($"    • Check-In:       {newCheckIn:MMMM dd, yyyy} (was {oldCheckIn:MMMM dd, yyyy})");
                hasChanges = true;
            }

            if (oldCheckOut != newCheckOut)
            {
                emailContent.AppendLine($"    • Check-Out:      {newCheckOut:MMMM dd, yyyy} (was {oldCheckOut:MMMM dd, yyyy})");
                hasChanges = true;
            }

            if (oldNumAdults != newNumAdults)
            {
                emailContent.AppendLine($"    • Adults:         {newNumAdults} (was {oldNumAdults})");
                hasChanges = true;
            }

            if (oldPets != newPets)
            {
                emailContent.AppendLine($"    • Pets:           {newPets} (was {oldPets})");
                hasChanges = true;
            }

            if (oldCost != newCost)
            {
                var difference = newCost - oldCost;
                emailContent.AppendLine($"    • Total Cost:     ${newCost:F2} (was ${oldCost:F2})");

                if (difference > 0)
                {
                    emailContent.AppendLine($"    • Additional:     ${difference:F2} charged");
                }
                else if (difference < 0)
                {
                    emailContent.AppendLine($"    • Refund:         ${Math.Abs(difference):F2}");
                }
                hasChanges = true;
            }

            if (!hasChanges)
            {
                emailContent.AppendLine("    • No changes detected");
            }

            emailContent.AppendLine();
            emailContent.AppendLine("  CURRENT RESERVATION:");
            emailContent.AppendLine($"    • Confirmation #: {reservationId}");
            emailContent.AppendLine($"    • Site:           {newSiteId}");
            emailContent.AppendLine($"    • Check-In:       {newCheckIn:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Check-Out:      {newCheckOut:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Adults:         {newNumAdults}");
            emailContent.AppendLine($"    • Pets:           {newPets}");
            emailContent.AppendLine($"    • Total Cost:     ${newCost:F2}");

            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine("  ✅ This email was NOT actually sent (using MockEmailService)");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝\n");

            _logger.LogInformation(emailContent.ToString());
            Console.WriteLine(emailContent.ToString());

            return Task.CompletedTask;
        }

        public Task SendReservationCanceledEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime checkInDate, DateTime checkOutDate, int siteId, decimal totalCost)
        {
            var emailContent = new StringBuilder();
            emailContent.AppendLine("\n╔════════════════════════════════════════════════════════════════╗");
            emailContent.AppendLine("║          ❌ RESERVATION CANCELED EMAIL (MOCK)                 ║");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            emailContent.AppendLine($"  To:      {toEmail}");
            emailContent.AppendLine($"  Subject: Reservation Canceled - Confirmation #{reservationId}");
            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine($"  Dear {customerName},");
            emailContent.AppendLine("  Your reservation has been canceled as requested.");
            emailContent.AppendLine();
            emailContent.AppendLine("  CANCELED RESERVATION:");
            emailContent.AppendLine($"    • Confirmation #: {reservationId}");
            emailContent.AppendLine($"    • Site Number:    {siteId}");
            emailContent.AppendLine($"    • Check-In:       {checkInDate:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Check-Out:      {checkOutDate:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Total Cost:     ${totalCost:F2}");
            emailContent.AppendLine($"    • Canceled:       {DateTime.Now:MMMM dd, yyyy}");
            emailContent.AppendLine();
            emailContent.AppendLine("  REFUND POLICY:");
            emailContent.AppendLine("    • 7+ days before:  Full refund");
            emailContent.AppendLine("    • 3-6 days before: 50% refund");
            emailContent.AppendLine("    • Less than 3 days: No refund");
            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine("  ✅ This email was NOT actually sent (using MockEmailService)");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝\n");

            _logger.LogInformation(emailContent.ToString());
            Console.WriteLine(emailContent.ToString());

            return Task.CompletedTask;
        }

        public Task SendCheckInReminderEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime checkInDate, int siteId, string extraNotes)
        {
            var emailContent = new StringBuilder();
            emailContent.AppendLine("\n╔════════════════════════════════════════════════════════════════╗");
            emailContent.AppendLine("║          🔔 CHECK-IN REMINDER EMAIL (MOCK)                    ║");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝");
            emailContent.AppendLine($"  To:      {toEmail}");
            emailContent.AppendLine($"  Subject: Check-In Reminder - Your Reservation is Coming Up!");
            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine($"  Dear {customerName},");
            emailContent.AppendLine("  Your reservation is coming up in 3 days!");
            emailContent.AppendLine();
            emailContent.AppendLine("  UPCOMING RESERVATION:");
            emailContent.AppendLine($"    • Confirmation #: {reservationId}");
            emailContent.AppendLine($"    • Check-In Date:  {checkInDate:MMMM dd, yyyy}");
            emailContent.AppendLine($"    • Site Number:    {siteId}");
            emailContent.AppendLine($"    • Check-In Time:  2:00 PM");
            emailContent.AppendLine();
            emailContent.AppendLine("  REMINDERS:");
            emailContent.AppendLine("    • Bring valid ID and confirmation number");
            emailContent.AppendLine("    • Check-in at main office");
            emailContent.AppendLine("    • Quiet hours: 10:00 PM - 7:00 AM");
            emailContent.AppendLine("    • Speed limit: 10 MPH");

            if (!string.IsNullOrWhiteSpace(extraNotes))
            {
                emailContent.AppendLine();
                emailContent.AppendLine($"  NOTES: {extraNotes}");
            }

            emailContent.AppendLine("─────────────────────────────────────────────────────────────────");
            emailContent.AppendLine("  ✅ This email was NOT actually sent (using MockEmailService)");
            emailContent.AppendLine("╚════════════════════════════════════════════════════════════════╝\n");

            _logger.LogInformation(emailContent.ToString());
            Console.WriteLine(emailContent.ToString());

            return Task.CompletedTask;
        }
    }
}
