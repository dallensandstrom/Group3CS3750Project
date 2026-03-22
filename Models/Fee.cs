using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<ReservationFee> ReservationFees { get; set; } = new();
        public List<SiteFee> SiteFees { get; set; } = new();

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

        public bool AppliesOnDate(DateTime date)
        {
            var day = date.Date;
            var startsOk = !StartDate.HasValue || StartDate.Value.Date <= day;
            var endsOk = !EndDate.HasValue || EndDate.Value.Date >= day;
            return startsOk && endsOk;
        }

        public Fee()
        {
        }
    }
}