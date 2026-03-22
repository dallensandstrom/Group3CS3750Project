using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class PriceRange
    {
        [Key]
        public int PriceRangeID { get; set; }

        [ForeignKey("SiteCategory")]
        public int SiteCategoryId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public SiteCategory? SiteCategory { get; set; }

        public bool IsInRange(DateTime date)
        {
            if (EndDate == null)
            {
                return date >= StartDate;
            }

            return date >= StartDate && date <= EndDate.Value;
        }

        public PriceRange()
        {

        }
    }
}