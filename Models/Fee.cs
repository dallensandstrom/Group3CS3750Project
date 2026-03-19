using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class Fee
    {
        [Key]
        public int FeeID { get; set; }

        [Required]
        public string Name { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public string AppliesTo { get; set; }

        public List<ReservationFee> ReservationFees { get; set; } = new();
    }
}