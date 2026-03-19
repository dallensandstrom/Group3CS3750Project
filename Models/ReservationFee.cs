using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class ReservationFee
    {
        [Key]
        public int ReservationFeeID { get; set; }

        [ForeignKey("Reservation")]
        public int ReservationID { get; set; }

        [ForeignKey("Fee")]
        public int FeeID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        // Navigation properties
        public Reservation? Reservation { get; set; }
        public Fee? Fee { get; set; }

        public ReservationFee()
        {

        }
    }
}
