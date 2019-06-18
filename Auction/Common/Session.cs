using System;
using Auction.DataAccess.Entities;

namespace Auction.Common
{
    public sealed class Session //singleton
    {
        private Session() { }

        private static readonly Lazy<Session> Lazy = new Lazy<Session>(() => new Session());

        public static Session CurrentSession => Lazy.Value; // при первом обращении создает сессию

        public static void SetCurrentUser(User user, bool isHandled = false)
        {
            CurrentSession.User = user;
            CurrentSession.CurrentUserUpdated?.Invoke(CurrentSession, new SessionEventArgs {IsHandled = isHandled});
        }

        public User User { get; private set; }

        public bool IsLoggedIn => User != null;

        public event EventHandler<SessionEventArgs> CurrentUserUpdated;
    }

    public class SessionEventArgs: EventArgs
    {
        public bool IsHandled { get; set; }
    }
}
