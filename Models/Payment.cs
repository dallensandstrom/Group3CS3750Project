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
        public int ReservationID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentType { get; set; }

        public string? StripePaymentID { get; set; }  // nullable

        public string Status { get; set; }

        [ValidateNever]
        public Reservation Reservation { get; set; }
    }
}