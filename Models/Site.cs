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

        [Column(TypeName = "decimal(10,2)")]
        public decimal DefaultPrice { get; set; }

        public List<SitePhoto> SitePhoto { get; set; } = new();
        public ICollection<Reservation>? Reservations { get; set; }

        public Site()
        {
            
        }
    }
}
