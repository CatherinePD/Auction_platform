using System;
using System.Threading.Tasks;
using Auction.Notification.EventsModels;

namespace Auction.Notification
{
    public interface IDataNotificator
    {
        event EventHandler<DataUpdatedEventArgs> OnDataUpdated;
        Task Listen();
        void Stop();
    }
}