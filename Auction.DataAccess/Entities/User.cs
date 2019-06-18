using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Auction.DataAccess.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(75)]
        public string Login { get; set; }

        public int? ContactId { get; set; }

        [Required]
        [StringLength(150)]
        public string Password { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public virtual Contact Contact { get; set; }

        public virtual ICollection<Lot> SelfLots { get; set; }

        public virtual ICollection<Lot> Lots { get; set; }
    }
}
