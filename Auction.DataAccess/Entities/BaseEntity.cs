using System.ComponentModel.DataAnnotations;

namespace Auction.DataAccess.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
