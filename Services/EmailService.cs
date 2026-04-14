using System.Net;
using System.Net.Mail;
using System.Text;

namespace GroupThreeTrailerParkProject.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPaymentConfirmationEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime checkInDate, DateTime checkOutDate, int siteId, 
            decimal totalCost, string extraNotes)
        {
            var subject = $"Reservation Confirmed - Confirmation #{reservationId}";

            var body = new StringBuilder();
            body.AppendLine($"<h2>Thank you for your reservation, {customerName}!</h2>");
            body.AppendLine($"<p>Your payment has been processed successfully and your reservation is confirmed.</p>");
            body.AppendLine("<hr/>");
            body.AppendLine("<h3>Reservation Details:</h3>");
            body.AppendLine("<ul>");
            body.AppendLine($"<li><strong>Confirmation Number:</strong> {reservationId}</li>");
            body.AppendLine($"<li><strong>Site Number:</strong> {siteId}</li>");
            body.AppendLine($"<li><strong>Check-In Date:</strong> {checkInDate:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Check-Out Date:</strong> {checkOutDate:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Total Cost:</strong> ${totalCost:F2}</li>");
            body.AppendLine("</ul>");

            if (!string.IsNullOrWhiteSpace(extraNotes))
            {
                body.AppendLine("<h3>Additional Information:</h3>");
                body.AppendLine($"<p>{extraNotes.Replace("\n", "<br/>")}</p>");
            }

            body.AppendLine("<hr/>");
            body.AppendLine("<p><strong>Important Reminders:</strong></p>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>Check-in time: 2:00 PM</li>");
            body.AppendLine("<li>Check-out time: 11:00 AM</li>");
            body.AppendLine("<li>Please bring a valid ID and your confirmation number</li>");
            body.AppendLine("<li>Quiet hours are from 10:00 PM to 7:00 AM</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<p>We look forward to seeing you!</p>");
            body.AppendLine("<p>If you need to make any changes, please log in to your account.</p>");

            await SendEmailAsync(toEmail, subject, body.ToString());
        }

        public async Task SendReservationModifiedEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime oldCheckIn, DateTime newCheckIn, DateTime oldCheckOut, 
            DateTime newCheckOut, int oldSiteId, int newSiteId, int oldNumAdults, int newNumAdults,
            int oldPets, int newPets, decimal oldCost, decimal newCost)
        {
            var subject = $"Reservation Updated - Confirmation #{reservationId}";

            var body = new StringBuilder();
            body.AppendLine($"<h2>Your reservation has been updated, {customerName}</h2>");
            body.AppendLine($"<p>We've processed the changes to your reservation.</p>");
            body.AppendLine("<hr/>");
            body.AppendLine("<h3>What Changed:</h3>");
            body.AppendLine("<ul>");

            if (oldSiteId != newSiteId)
            {
                body.AppendLine($"<li><strong>Site Number:</strong> {newSiteId} <span style='color: gray;'>(was {oldSiteId})</span></li>");
            }

            if (oldCheckIn != newCheckIn)
            {
                body.AppendLine($"<li><strong>Check-In Date:</strong> {newCheckIn:MMMM dd, yyyy} <span style='color: gray;'>(was {oldCheckIn:MMMM dd, yyyy})</span></li>");
            }

            if (oldCheckOut != newCheckOut)
            {
                body.AppendLine($"<li><strong>Check-Out Date:</strong> {newCheckOut:MMMM dd, yyyy} <span style='color: gray;'>(was {oldCheckOut:MMMM dd, yyyy})</span></li>");
            }

            if (oldNumAdults != newNumAdults)
            {
                body.AppendLine($"<li><strong>Number of Adults:</strong> {newNumAdults} <span style='color: gray;'>(was {oldNumAdults})</span></li>");
            }

            if (oldPets != newPets)
            {
                body.AppendLine($"<li><strong>Number of Pets:</strong> {newPets} <span style='color: gray;'>(was {oldPets})</span></li>");
            }

            if (oldCost != newCost)
            {
                var difference = newCost - oldCost;
                body.AppendLine($"<li><strong>Total Cost:</strong> ${newCost:F2} <span style='color: gray;'>(was ${oldCost:F2})</span></li>");
            }

            body.AppendLine("</ul>");

            body.AppendLine("<h3>Current Reservation Details:</h3>");
            body.AppendLine("<ul>");
            body.AppendLine($"<li><strong>Confirmation Number:</strong> {reservationId}</li>");
            body.AppendLine($"<li><strong>Site Number:</strong> {newSiteId}</li>");
            body.AppendLine($"<li><strong>Check-In Date:</strong> {newCheckIn:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Check-Out Date:</strong> {newCheckOut:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Adults:</strong> {newNumAdults}</li>");
            body.AppendLine($"<li><strong>Pets:</strong> {newPets}</li>");
            body.AppendLine($"<li><strong>Total Cost:</strong> ${newCost:F2}</li>");
            body.AppendLine("</ul>");

            if (oldCost != newCost)
            {
                body.AppendLine("<h3>Payment Update:</h3>");
                var difference = newCost - oldCost;
                if (difference > 0)
                {
                    body.AppendLine($"<p style='color: green;'>An additional payment of <strong>${difference:F2}</strong> has been processed.</p>");
                }
                else if (difference < 0)
                {
                    body.AppendLine($"<p style='color: blue;'>A refund of <strong>${Math.Abs(difference):F2}</strong> will be processed within 3-5 business days.</p>");
                }
            }

            body.AppendLine("<hr/>");
            body.AppendLine("<p>If you have any questions about these changes, please contact us.</p>");

            await SendEmailAsync(toEmail, subject, body.ToString());
        }

        public async Task SendReservationCanceledEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime checkInDate, DateTime checkOutDate, int siteId, decimal totalCost)
        {
            var subject = $"Reservation Canceled - Confirmation #{reservationId}";

            var body = new StringBuilder();
            body.AppendLine($"<h2>Reservation Canceled</h2>");
            body.AppendLine($"<p>Dear {customerName},</p>");
            body.AppendLine($"<p>Your reservation has been canceled as requested.</p>");
            body.AppendLine("<hr/>");
            body.AppendLine("<h3>Canceled Reservation Details:</h3>");
            body.AppendLine("<ul>");
            body.AppendLine($"<li><strong>Confirmation Number:</strong> {reservationId}</li>");
            body.AppendLine($"<li><strong>Site Number:</strong> {siteId}</li>");
            body.AppendLine($"<li><strong>Check-In Date:</strong> {checkInDate:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Check-Out Date:</strong> {checkOutDate:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Total Cost:</strong> ${totalCost:F2}</li>");
            body.AppendLine($"<li><strong>Cancellation Date:</strong> {DateTime.Now:MMMM dd, yyyy}</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<hr/>");
            body.AppendLine("<h3>Refund Information:</h3>");
            body.AppendLine("<p>Your refund will be processed according to our cancellation policy:</p>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>Cancellations made 7+ days before check-in: Full refund</li>");
            body.AppendLine("<li>Cancellations made 3-6 days before check-in: 50% refund</li>");
            body.AppendLine("<li>Cancellations made less than 3 days before check-in: No refund</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<p>Refunds typically process within 5-7 business days.</p>");
            body.AppendLine("<hr/>");
            body.AppendLine("<p>We're sorry to see your plans change. We hope to welcome you in the future!</p>");
            body.AppendLine("<p>If you have any questions, please don't hesitate to contact us.</p>");

            await SendEmailAsync(toEmail, subject, body.ToString());
        }

        public async Task SendCheckInReminderEmailAsync(string toEmail, string customerName, 
            int reservationId, DateTime checkInDate, int siteId, string extraNotes)
        {
            var subject = $"Check-In Reminder - Your Reservation is Coming Up!";

            var body = new StringBuilder();
            body.AppendLine($"<h2>Your Reservation is Almost Here!</h2>");
            body.AppendLine($"<p>Dear {customerName},</p>");
            body.AppendLine($"<p>We're looking forward to welcoming you in just a few days!</p>");
            body.AppendLine("<hr/>");
            body.AppendLine("<h3>Your Upcoming Reservation:</h3>");
            body.AppendLine("<ul>");
            body.AppendLine($"<li><strong>Confirmation Number:</strong> {reservationId}</li>");
            body.AppendLine($"<li><strong>Check-In Date:</strong> {checkInDate:MMMM dd, yyyy}</li>");
            body.AppendLine($"<li><strong>Site Number:</strong> {siteId}</li>");
            body.AppendLine($"<li><strong>Check-In Time:</strong> 2:00 PM</li>");
            body.AppendLine("</ul>");

            if (!string.IsNullOrWhiteSpace(extraNotes))
            {
                body.AppendLine("<h3>Special Notes:</h3>");
                body.AppendLine($"<p>{extraNotes.Replace("\n", "<br/>")}</p>");
            }

            body.AppendLine("<hr/>");
            body.AppendLine("<h3>Important Reminders:</h3>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>Bring a valid ID and your confirmation number</li>");
            body.AppendLine("<li>Check-in at the main office upon arrival</li>");
            body.AppendLine("<li>Review park rules and regulations</li>");
            body.AppendLine("<li>Quiet hours: 10:00 PM - 7:00 AM</li>");
            body.AppendLine("<li>Maximum speed limit: 10 MPH</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<h3>What to Bring:</h3>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>RV/camping gear</li>");
            body.AppendLine("<li>Power adapters (30/50 amp)</li>");
            body.AppendLine("<li>Water hose</li>");
            body.AppendLine("<li>Sewer hose and connections</li>");
            body.AppendLine("<li>Pet supplies (if applicable)</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<hr/>");
            body.AppendLine("<p><strong>Need to make changes?</strong> Please log in to your account or contact us as soon as possible.</p>");
            body.AppendLine("<p>We can't wait to see you!</p>");

            await SendEmailAsync(toEmail, subject, body.ToString());
        }

        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPass = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {toEmail} - Subject: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail} - Subject: {subject}");
                // Don't throw exception - don't want email failures to break the app
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string firstName, string lastName)
        {
            var subject = "Welcome to RV Park!";
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9;'>
                        <h2 style='color: #2c5f2d; border-bottom: 3px solid #2c5f2d; padding-bottom: 10px;'>
                            Welcome to RV Park!
                        </h2>

                        <p>Dear {firstName} {lastName},</p>

                        <p>Welcome to RV Park! Your account has been successfully created and you're all set to start making reservations.</p>

                        <div style='background-color: white; padding: 20px; margin: 20px 0; border-radius: 5px; border-left: 4px solid #2c5f2d;'>
                            <h3 style='margin-top: 0; color: #2c5f2d;'>Getting Started:</h3>
                            <ul>
                                <li>Browse our available sites</li>
                                <li>Make your first reservation</li>
                                <li>Manage your bookings online anytime</li>
                                <li>View your reservation history</li>
                            </ul>
                        </div>

                        <p>If you have any questions or need assistance, please don't hesitate to contact us.</p>

                        <p style='margin-top: 30px;'>
                            Best regards,<br>
                            <strong>RV Park Team</strong>
                        </p>

                        <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #666;'>
                            <p>This is an automated email. Please do not reply to this message.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }
    }
}
