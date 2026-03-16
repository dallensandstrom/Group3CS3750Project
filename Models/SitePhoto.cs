using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class SitePhoto
    {
        [Key]
        public int PhotoId { get; set; }

        [ForeignKey("Site")]
        public int SiteId { get; set; }

        public required string PhotoUrl { get; set; }

        public SitePhoto()
        {
            
        }
    }
}
