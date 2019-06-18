using System;
using System.Diagnostics;
using Auction.DataAccess.Entities;

namespace Auction.DataAccess.UnitOfWork
{
    public class AuctionUnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly string _connectionString = ConnectionConstants.ConnectionString;
        private readonly AuctionContext _context;

        private IRepository<Bid> _bidRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Contact> _contactRepository;
        private IRepository<Lot> _lotRepository;
        private IRepository<LotContent> _lotContentRepository;
        private IRepository<User> _userRepository;
        private IRepository<Notification> _notificationRepository;

        public AuctionUnitOfWork()
        {
            _context = new AuctionContext(_connectionString);
            _context.Database.Log = s => Debug.WriteLine(s);
        }

        public IRepository<Bid> BidRepository =>
            _bidRepository ?? (_bidRepository = new AuctionRepository<Bid>(_context)); // проверка на null

        public IRepository<Category> CategoryRepository =>
            _categoryRepository ?? (_categoryRepository = new AuctionRepository<Category>(_context));

        public IRepository<Contact> ContactRepository =>
            _contactRepository ?? (_contactRepository = new AuctionRepository<Contact>(_context));

        public IRepository<Lot> LotRepository =>
            _lotRepository ?? (_lotRepository = new AuctionRepository<Lot>(_context));

        public IRepository<LotContent> LotContentRepository =>
            _lotContentRepository ?? (_lotContentRepository = new AuctionRepository<LotContent>(_context));

        public IRepository<User> UserRepository =>
            _userRepository ?? (_userRepository = new AuctionRepository<User>(_context));

        public IRepository<Notification> NotificationRepository =>
            _notificationRepository ?? (_notificationRepository = new AuctionRepository<Notification>(_context));

        public void Dispose()
        {
            _context.Dispose();
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}
