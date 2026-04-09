using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
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

        public Reservation? Reservation { get; set; }

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
            // Mark payment as completed
            Status = "Completed";
            PaymentDate = DateTime.Now;
            await Task.CompletedTask;
            return true;
        }

        public static async Task<Payment?> GetByReservationId(int reservationId, DbContext context)
        {
            // Fetch payment by reservation ID
            var payment = await context.Set<Payment>()
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
            return payment;
        }

        public Payment()
        {
            PaymentDate = DateTime.Now;
        }
    }
}
