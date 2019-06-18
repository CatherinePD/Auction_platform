using System;
using System.Xml.Serialization;

namespace Auction.Notification.EventsModels
{
    [Serializable]
    [XmlRoot("root")]
    public class NotificationData<T>
    {
        [XmlArray("inserted")]
        [XmlArrayItem("row")]
        public T[] Inserted { get; set; }

        [XmlArray("deleted")]
        [XmlArrayItem("row")]
        public T[] Deleted { get; set; }

        [XmlArray("updated")]
        [XmlArrayItem("row")]
        public T[] Updated { get; set; }
    }
}
