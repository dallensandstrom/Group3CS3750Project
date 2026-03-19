using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class PriceRange
    {
        [Key]
        public int PriceRangeID { get; set; }

        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Navigation property
        public Site? Site { get; set; }

        // Method from class diagram
        public bool IsInRange(DateTime date)
        {
            // If EndDate is null, this is the current/default price (applies indefinitely from StartDate)
            if (EndDate == null)
            {
                return date >= StartDate;
            }

            // Otherwise check if date falls within the specific range
            return date >= StartDate && date <= EndDate;
        }

        public PriceRange()
        {

        }
    }
}
