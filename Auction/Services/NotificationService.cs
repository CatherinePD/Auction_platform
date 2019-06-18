using System.Collections.Generic;
using System.Linq;
using Auction.DataAccess.UnitOfWork;
using NotificationEntity = Auction.DataAccess.Entities.Notification;

namespace Auction.Services
{
    public class NotificationService
    {
        public void Send(NotificationEntity notification, AuctionUnitOfWork unitOfWork)
        {
            unitOfWork.NotificationRepository.Add(notification);
        }

        public ICollection<NotificationEntity> GetUserNotifications(int userId)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var notifications = unitOfWork.NotificationRepository.Get(n => n.RecieverId == userId && n.IsReaded == false);
                return notifications;
            }
        }

        public ICollection<NotificationEntity> MarkAsReaded(IEnumerable<int> notificationIDs)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var notifications = unitOfWork.NotificationRepository.Get(n => notificationIDs.Contains(n.Id));

                foreach (var notification in notifications)
                    notification.IsReaded = true;

                unitOfWork.Commit();

                return notifications;
            }
        }
    }
}
