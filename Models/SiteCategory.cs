using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class SiteCategory
    {
        [Key]
        public int SiteCategoryId { get; set; }

        public required string Name { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerWeek { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerMonth { get; set; }

        public SiteCategory()
        {
            
        }
    }
}
