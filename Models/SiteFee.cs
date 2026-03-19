using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupThreeTrailerParkProject.Models
{
    public class SiteFee
    {
        [Key]
        public int SiteFeeID { get; set; }

        [ForeignKey("Fee")]
        public int FeeID { get; set; }

        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [ValidateNever]
        public Fee Fee { get; set; }

        [ValidateNever]
        public Site Site { get; set; }
    }
}