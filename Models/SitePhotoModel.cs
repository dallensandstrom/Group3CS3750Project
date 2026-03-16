using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class SitePhotoModel
    {
        [Key]
        public int PhotoId { get; set; }

        public int SiteId { get; set; }

        public required string PhotoUrl { get; set; }

        public SitePhotoModel()
        {
            
        }
    }
}
