using System.ComponentModel.DataAnnotations;

namespace Auction.DataAccess.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
