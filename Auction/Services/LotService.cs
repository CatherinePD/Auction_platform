using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Auction.DataAccess.Entities;
using Auction.DataAccess.UnitOfWork;
using LotEntity = Auction.DataAccess.Entities.Lot;
using NotificationEntity = Auction.DataAccess.Entities.Notification;

namespace Auction.Services
{
    public class LotService
    {
        private NotificationService _notificationService;

        public LotService()
        {
            _notificationService = new NotificationService();
        }

        public void AddLot(LotEntity lot)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                unitOfWork.LotRepository.Add(lot);
                unitOfWork.Commit();
            }
        }

        public void UpdateLot(LotEntity lot, bool contentChanged)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var dbLot = unitOfWork.LotRepository.Get(lot.Id);

                if (contentChanged)
                {
                    var dbContents = unitOfWork.LotContentRepository.Get(c => c.LotId == lot.Id);

                    foreach (var content in dbContents)
                    {
                        unitOfWork.LotContentRepository.Delete(content);
                    }

                    var cnt = lot.LotContents.Select(c => new LotContent {Content = c.Content, LotId = c.LotId}).ToList();
                    dbLot.LotContents = cnt;
                }

                dbLot.Title = lot.Title;
                dbLot.Description = lot.Description;
                dbLot.CategoryId = lot.CategoryId;

                unitOfWork.Commit();
            }
        }

        public LotEntity GetLot(int id)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var lot = unitOfWork.LotRepository.Select()
                    .Include(l => l.Category)
                    .Include(l => l.Bids)
                    .Include(l => l.LotContents)
                    .Include(l => l.Users)
                    .Include(l => l.Owner)
                    .Include(l => l.Owner.Contact)
                    .FirstOrDefault(l => l.Id == id);

                return lot;
            }
        }

        public void AddBid(Bid bid)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var lot = unitOfWork.LotRepository.Get(bid.LotId);
                var user = unitOfWork.UserRepository.Get(bid.UserId);

                lot.Bids.Add(bid);

                if (!lot.Users.Any(u => u.Id == bid.UserId))
                    lot.Users.Add(user);

                lot.CurrentBid = bid.Amount;

                unitOfWork.Commit();
            }
        }

        public void DisableLot(int lotId)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var lot = unitOfWork.LotRepository.Get(lotId);

                lot.IsActive = false;
                lot.DateFinished = DateTime.Now;

                Notify(unitOfWork, lot);

                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// завершение лотов, у которых время торгов истекло, при старте приложения
        /// </summary>
        public void RunLotExpirationProcessor()
        {
            Task.Factory.StartNew(() =>
            {
                using (var unitOfWork = new AuctionUnitOfWork())
                {
                    var lotsToExpire = unitOfWork.LotRepository.Select()
                        .Where(l => l.IsActive && l.DateToExpire < DateTime.Now).ToList();

                    foreach (var lot in lotsToExpire)
                    {
                        lot.IsActive = false;
                        lot.DateFinished = DateTime.Now;

                        Notify(unitOfWork, lot);
                    }

                    unitOfWork.Commit();
                }
            }).ConfigureAwait(false);
        }
        /// <summary>
        /// оповещает каждого пользователя, участвующего в торгах по лоту, о его завершении
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="lot"></param>
        private void Notify(AuctionUnitOfWork unitOfWork, LotEntity lot)
        {
            var toOwner = BuildOwnerNotification(lot);
            _notificationService.Send(toOwner, unitOfWork);

            if (lot.Bids == null || !lot.Bids.Any()) return;

            var maxBid = lot.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
            var winner = maxBid.User;

            var toWinner = BuildWinnerNotification(maxBid.UserId, lot);
            _notificationService.Send(toWinner, unitOfWork);

            foreach (var user in lot.Users.Where(u => u.Id != winner.Id))
            {
                var toUser = BuildUserNotification(user.Id, lot);
                _notificationService.Send(toUser, unitOfWork);
            }
        }

        private NotificationEntity BuildLotNotification()
        {
            var result = new NotificationEntity
            {
                DateCreated = DateTime.Now,
                IsReaded = false,
                Source = EventSourceEnum.Lot
            };

            return result;
        }

        private NotificationEntity BuildWinnerNotification(int userId, LotEntity lot)
        {
            var toWinner = BuildLotNotification();

            toWinner.RecieverId = userId;
            toWinner.EntityId = lot.Id;
            toWinner.Message = $"Торги по лоту \"{lot.Title}\" завершились. Ваша ставка была максимальной. Свяжитесь с продавцом для покупки товара.";

            return toWinner;
        }

        private NotificationEntity BuildOwnerNotification(LotEntity lot)
        {
            var toOwner = BuildLotNotification();

            toOwner.RecieverId = lot.OwnerId;
            toOwner.EntityId = lot.Id;
            toOwner.Message = $"Созданный вами аукцион \"{lot.Title}\" закрыт";

            return toOwner;
        }

        private NotificationEntity BuildUserNotification(int userId, LotEntity lot)
        {
            var toUser = BuildLotNotification();

            toUser.RecieverId = userId;
            toUser.EntityId = lot.Id;
            toUser.Message = $"Торги по лоту \"{lot.Title}\" завершились. К сожалению, ваша ставка не была максимальной";

            return toUser;
        }
    }
}
