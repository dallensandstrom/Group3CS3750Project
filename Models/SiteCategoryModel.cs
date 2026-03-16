namespace GroupThreeTrailerParkProject.Models
{
    public class SiteCategoryModel
    {
        public int SiteCategoryId { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PricePerWeek { get; set; }
        public decimal PricePerMonth { get; set; }

        public SiteCategoryModel()
        {
            
        }
    }
}
