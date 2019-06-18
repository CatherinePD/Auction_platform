using System.ComponentModel.DataAnnotations;

namespace Auction.DataAccess.Entities
{
    public class LotContent : BaseEntity
    {
        [Required]
        public byte[] Content { get; set; }

        public int LotId { get; set; }

        public virtual Lot Lot { get; set; }
    }
}
