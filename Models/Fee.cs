using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class Fee
    {
        [Key]
        public int FeeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string AppliesTo { get; set; } = string.Empty;

        // Navigation properties
        public List<ReservationFee> ReservationFees { get; set; } = new();
        public List<SiteFee> SiteFees { get; set; } = new();

        // Methods from class diagram
        public bool AppliesToReservation()
        {
            return AppliesTo.Equals("Reservation", StringComparison.OrdinalIgnoreCase) ||
                   AppliesTo.Contains("Reservation", StringComparison.OrdinalIgnoreCase);
        }

        public bool AppliesToSite()
        {
            return AppliesTo.Equals("Site", StringComparison.OrdinalIgnoreCase) ||
                   AppliesTo.Contains("Site", StringComparison.OrdinalIgnoreCase);
        }

        public Fee()
        {

        }
    }
}
