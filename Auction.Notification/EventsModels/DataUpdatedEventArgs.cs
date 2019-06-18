using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Auction.Notification.EventsModels
{
    public class DataUpdatedEventArgs: EventArgs
    {
        public XElement Data { get; set; }
        public ActionType Action { get; set; }

        public NotificationData<T> GetNotificationObject<T>()
        {
            return Deserialize<T>(Data);
        }

        private NotificationData<T> Deserialize<T>(XNode xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NotificationData<T>));

            try
            {
                using (var reader = xml.CreateReader())
                {
                    var obj = (NotificationData<T>)serializer.Deserialize(reader);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    [Flags]
    public enum ActionType
    {
        None = 0,
        Insert = 1 << 1,
        Update = 1 << 2,
        Delete = 1 << 3
    }
}
