using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


//Done by Dallen
namespace GroupThreeTrailerParkProject.Models
{
    public class GuestProfile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserAccount")]
        public string UserAccountID { get; set; } = default!;
        public UserAccount UserAccount { get; set; } = default!;

        [Required]
        public DODAffiliation? DODAffiliation { get; set; }
        [Required]
        public DODStatus? DODStatus { get; set; }
    }
    public enum DODAffiliation
    {
        Married,
        Parent,
        Child,
        Other
    }

    public enum DODStatus
    {
        [Display(Name = "Active Duty")]
        ActiveDuty,
        Reservist,
        Retired,
        [Display(Name = "On PCS Orders")]
        OnPCSOrders
    }
}
