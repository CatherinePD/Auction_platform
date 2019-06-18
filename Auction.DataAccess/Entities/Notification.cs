using System;
using System.Xml.Serialization;

namespace Auction.DataAccess.Entities
{
    [Serializable]
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public int? RecieverId { get; set; }
        public bool IsReaded { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateReaded { get; set; }
        public virtual EventSourceEnum Source { get; set; }
        public int? EntityId { get; set; }

        [XmlIgnore]
        public User Reciever { get; set; }
    }
}