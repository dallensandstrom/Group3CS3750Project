using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class SiteModel
    {
        [Key]
        public int SiteId { get; set; }

        public int SiteCategoryId { get; set; }

        public int MaxVehicleSize { get; set; }

        public bool VisibleToClient { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DefaultPrice { get; set; }

        public SiteModel()
        {
            
        }
    }
}
