using System.ComponentModel.DataAnnotations;

namespace GroupThreeTrailerParkProject.Models
{
    public class SiteCategory
    {
        [Key]
        public int SiteCategoryId { get; set; }

        public required string Name { get; set; }

        public List<PriceRange> PriceRanges { get; set; } = new();

        public SiteCategory()
        {

        }
    }
}