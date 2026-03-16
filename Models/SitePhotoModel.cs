namespace GroupThreeTrailerParkProject.Models
{
    public class SitePhotoModel
    {
        public int PhotoId { get; set; }
        public int SiteId { get; set; }
        public required string PhotoUrl { get; set; }

        public SitePhotoModel()
        {
            
        }
    }
}
