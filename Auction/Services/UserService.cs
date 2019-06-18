using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using Auction.Common;
using Auction.DataAccess.Entities;
using Auction.DataAccess.UnitOfWork;

namespace Auction.Services
{
    public class UserService
    {
        private readonly Session _session;
        public UserService()
        {
            _session = Session.CurrentSession;
        }

        public User GetUser(Func<User, bool> predicate)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var user = unitOfWork.UserRepository.Select()
                    .Include(u => u.Contact)
                    .FirstOrDefault(predicate);
                return user;
            }
        }

        public bool TryAuthenticate(string login, string password, out User user)
        {
            user = GetUser(u => string.Equals(u.Login, login) && 
                                string.Equals(u.Password, password));

            return user != null;
        }

        public void LogOut()
        {
            Session.SetCurrentUser(null);
        }

        public bool TryRegistration(User user, string passwordConfirm,  out string message)
        {
            message = "OK";

            if (user.Password != passwordConfirm)
            {
                message = "Корректно подтвердите пароль!";
                return false;
            }

            using (var unitOfWork = new AuctionUnitOfWork())
            {
                if (unitOfWork.UserRepository.Get(u => u.Login == user.Login).Any())
                {
                    message = "Пользователь с таким логином уже существует";
                    return false;
                }
                unitOfWork.UserRepository.Add(user);
                unitOfWork.Commit();
            }
            return true;
        }

        public void ChangePassword(int userId, string newPassword)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                var user = unitOfWork.UserRepository.Get(userId);
                user.Password = newPassword;
                unitOfWork.UserRepository.Update(user);
                unitOfWork.Commit();
            }
        }

        public bool ChangeLogin(int userId, string newLogin)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                if (unitOfWork.UserRepository.Get(u => u.Login == newLogin).Any())
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Логин", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                var user = unitOfWork.UserRepository.Get(userId);
                user.Login = newLogin;
                unitOfWork.UserRepository.Update(user);
                unitOfWork.Commit();
            }
            return true;
        }

        public void AddContact(Contact contact, int userId)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                unitOfWork.ContactRepository.Add(contact);

                var user = unitOfWork.UserRepository.Get(userId);
                user.ContactId = contact.Id;
                user.Contact = contact;

                unitOfWork.Commit();
            }
        }

        public void UpdateContact(Contact contact)
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                unitOfWork.ContactRepository.Update(contact);
                unitOfWork.Commit();
            }
        }
    }
}
