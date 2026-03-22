using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class Site
    {
        [Key]
        public int SiteId { get; set; }

        [ForeignKey("SiteCategory")]
        public int SiteCategoryId { get; set; }

        public int MaxVehicleSize { get; set; }

        public bool VisibleToClient { get; set; }

        // Navigation properties
        public SiteCategory? SiteCategory { get; set; }
        public List<SitePhoto> SitePhoto { get; set; } = new();
        public List<SiteFee> SiteFees { get; set; } = new();
        public ICollection<Reservation>? Reservations { get; set; }

        public Site()
        {

        }
    }
}