using Auction.DataAccess.Entities;
using System.Data.Entity;

namespace Auction.DataAccess
{
    public class AuctionContext : DbContext
    {
        public AuctionContext(string connectionString)
            :base(connectionString)
        {
            Database.SetInitializer(new DatabaseInitializer());
        }
        public AuctionContext()
            : this("name=Watchman")
        {
        }

        public virtual DbSet<Bid> Bids { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<LotContent> LotContents { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Lot
            modelBuilder.Entity<Lot>()
                .HasMany(l => l.Bids)
                .WithRequired(b => b.Lot)
                .HasForeignKey(e => e.LotId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Lot>()
                .HasMany(l => l.Users)
                .WithMany(u => u.Lots)
                .Map(m =>
                {
                    m.ToTable("UserLots");
                    m.MapLeftKey("LotId");
                    m.MapRightKey("UserId");
                });

            modelBuilder.Entity<Lot>()
                .HasMany(l => l.LotContents)
                .WithRequired(lc => lc.Lot)
                .HasForeignKey(lc => lc.LotId)
                .WillCascadeOnDelete(false);
            #endregion

            #region User
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bids)
                .WithRequired(b => b.User)
                .HasForeignKey(b => b.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SelfLots)
                .WithRequired(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .WillCascadeOnDelete(false);
            #endregion

            modelBuilder.Entity<Notification>()
                .HasOptional(n => n.Reciever);

            base.OnModelCreating(modelBuilder);
        }
    }
}
