namespace GroupThreeTrailerParkProject.Models
{
    public class SiteModel
    {
        public int SiteId { get; set; }
        public int SiteCategoryId { get; set; }
        public int MaxVehicleSize { get; set; }
        public bool VisibleToClient { get; set; }
        public decimal DefaultPrice { get; set; }

        public SiteModel()
        {
            
        }
    }
}
