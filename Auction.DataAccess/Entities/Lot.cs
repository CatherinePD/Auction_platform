using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Auction.DataAccess.Entities
{
    public class Lot : BaseEntity
    {
        [Required]
        [StringLength(75)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateToExpire { get; set; }

        public DateTime? DateFinished { get; set; }

        public decimal StartBid { get; set; }

        public decimal CurrentBid { get; set; }

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public int OwnerId { get; set; }

        public User Owner { get; set; }

        public virtual ICollection<LotContent> LotContents { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
