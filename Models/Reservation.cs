using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupThreeTrailerParkProject.Models;

namespace GroupThreeTrailerParkProject.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        public int AccountID { get; set; }
        public int SiteNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public int NumAdults { get; set; }

        public int Pets { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseCost { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCost { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public string ExtraNotes { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        // Navigation properties
        public List<ReservationFee> ReservationFees { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }
}