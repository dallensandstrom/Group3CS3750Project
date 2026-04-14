namespace GroupThreeTrailerParkProject.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a payment confirmation email when a reservation is confirmed
        /// </summary>
        Task SendPaymentConfirmationEmailAsync(string toEmail, string customerName, int reservationId, 
            DateTime checkInDate, DateTime checkOutDate, int siteId, decimal totalCost, string extraNotes);

        /// <summary>
        /// Sends an email when a confirmed reservation is modified
        /// </summary>
        Task SendReservationModifiedEmailAsync(string toEmail, string customerName, int reservationId,
            DateTime oldCheckIn, DateTime newCheckIn, DateTime oldCheckOut, DateTime newCheckOut,
            int oldSiteId, int newSiteId, int oldNumAdults, int newNumAdults, 
            int oldPets, int newPets, decimal oldCost, decimal newCost);

        /// <summary>
        /// Sends an email when a reservation is canceled
        /// </summary>
        Task SendReservationCanceledEmailAsync(string toEmail, string customerName, int reservationId,
            DateTime checkInDate, DateTime checkOutDate, int siteId, decimal totalCost);

        /// <summary>
        /// Sends a check-in reminder email 2-3 days before arrival
        /// </summary>
        Task SendCheckInReminderEmailAsync(string toEmail, string customerName, int reservationId,
            DateTime checkInDate, int siteId, string extraNotes);

        /// <summary>
        /// Sends a welcome email when a new account is registered
        /// </summary>
        Task SendWelcomeEmailAsync(string toEmail, string firstName, string lastName);
    }
}
