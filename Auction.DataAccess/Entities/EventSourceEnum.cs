using System.Xml.Serialization;

namespace Auction.DataAccess.Entities
{
    public enum EventSourceEnum: byte
    {
        [XmlEnum("0")]
        Default = 0,

        [XmlEnum("1")]
        Lot = 1,

        [XmlEnum("2")]
        Bid = 2
    }
}