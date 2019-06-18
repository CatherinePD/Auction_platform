using System;

namespace Auction.DataAccess.Entities
{
    public class Bid : BaseEntity
    {
        public decimal Amount { get; set; }

        public int LotId { get; set; }

        public int UserId { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual Lot Lot { get; set; }

        public virtual User User { get; set; }
    }
}
