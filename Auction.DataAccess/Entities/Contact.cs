using System;
using System.ComponentModel.DataAnnotations;

namespace Auction.DataAccess.Entities
{
    public class Contact : BaseEntity
    {
        [Required]
        [StringLength(75)]
        public string Email { get; set; }

        [StringLength(75)]
        public string Address { get; set; }

        [StringLength(75)]
        public string Phone { get; set; }

        public byte[] Photo { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
