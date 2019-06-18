using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Auction.DataAccess.Entities;
using Auction.DataAccess.UnitOfWork;
using Auction.Notification;
using User = Auction.DataAccess.Entities.User;

namespace Auction.SandBox
{
    class Program
    {
        //private const string connectionString = @"Data Source=(localDB)\MSSQLLocalDB;Initial Catalog=TestDB;Integrated Security=True";
        static void Main(string[] args)
        {
            //Init();
            NotificationTest();
        }

        private static void Init()
        {
            using (var uow = new AuctionUnitOfWork())
            {
                var cat = new Category { Description = "Default Description", Name = "Awesome" };
                var catR = uow.CategoryRepository;
                catR.Add(cat);

                var cnt = new Contact { Address = "Jaos", Email = "rxt@ash", Phone = "1230-112" };
                var cnt2 = new Contact { Address = "Jaosinen", Email = "rxqwrt@ash", Phone = "14130-112" };
                var cnt3 = new Contact { Address = "Bel", Email = "124t@as4", Phone = "112413()1312" };
                var contactR = uow.ContactRepository;
                contactR.Add(cnt);
                contactR.Add(cnt2);
                contactR.Add(cnt3);

                var usr = new User { Contact = cnt, ContactId = cnt.Id, Login = "Kate", Password = "1222" };
                var usr2 = new User { Contact = cnt2, ContactId = cnt2.Id, Login = "Jake", Password = "Crrr" };
                var userR = uow.UserRepository;
                userR.Add(usr);
                userR.Add(usr2);

                var lot = new Lot
                {
                    Users = new HashSet<User> { usr, usr2 },
                    Title = "GOOD TV",
                    StartBid = (decimal)123.21,
                    CurrentBid = (decimal)123.21,
                    DateCreated = DateTime.Now,
                    DateToExpire = new DateTime(2019, 11, 11),
                    Category = cat,
                    OwnerId = usr.Id,
                    Owner = usr,
                    Description = "Lorem ipsum dolor"
                };

                var lotR = uow.LotRepository;
                lotR.Add(lot);

                uow.Commit();
            }
        }

        private static void NotificationTest()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Lobster"].ConnectionString;

            DataNotificator.Register(connectionString, "TestDB", "Notifications");
            var notificator = DataNotificator.Instance;

            notificator.OnDataUpdated += (s, e) =>
            {
                Console.WriteLine("Your table was changed!");
                var x = e.GetNotificationObject<DataAccess.Entities.Notification>();
            };

            notificator.Listen();
            Console.WriteLine("Listener run!");

            Console.ReadLine();

            notificator.Stop();
        }
    }
}
