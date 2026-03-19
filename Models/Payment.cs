using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentType { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? StripePaymentID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // Navigation property
        public Reservation? Reservation { get; set; }

        // Methods from class diagram
        public void Create()
        {
            PaymentDate = DateTime.Now;
            Status = "Pending";
        }

        public bool Verify()
        {
            return Amount > 0 && !string.IsNullOrEmpty(PaymentType);
        }

        public async Task<bool> ProcessPayment()
        {
            // TODO: Implement Stripe payment processing
            // This will be integrated with Stripe API
            await Task.CompletedTask;
            return false;
        }

        public static async Task<Payment?> GetByReservationId(int reservationId)
        {
            // TODO: This will use repository pattern to fetch payment
            await Task.CompletedTask;
            return null;
        }

        public Payment()
        {
            PaymentDate = DateTime.Now;
        }
    }
}
