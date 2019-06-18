using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Auction.Common;
using Auction.DataAccess.UnitOfWork;
using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction.Controls
{
    /// <summary>
    /// Логика взаимодействия для LotList.xaml
    /// </summary>
    public partial class LotList : UserControl, INotifyPropertyChanged
    {
        private readonly Session _session;
        public LotList()
        {
            InitializeComponent();
            DataContext = this;
            _session = Session.CurrentSession;
            _session.CurrentUserUpdated += SessionOnCurrentUserUpdated;
        }

        private void SessionOnCurrentUserUpdated(object sender, SessionEventArgs sessionEventArgs)
        {
            OnPropertyChanged(nameof(UserName));
        }

        public string UserName => !_session.IsLoggedIn ? null : _session.User.Login;

        public void Refresh(Expression<Func<LotEntity, bool>> predicate = null, string filterMessage = null)
        {
            if (predicate == null)
                predicate = l => l.IsActive;

            FilterTextBlock.Text = filterMessage;

            lotStackPanel.Children.Clear();

            using (var uow = new AuctionUnitOfWork())
            {
                var lots = uow.LotRepository.Select()
                                            .Where(predicate)
                                            .OrderByDescending(l => l.DateCreated).ToList();

                lots.ForEach(lot =>
                {
                    var card = new LotCard
                    {
                        Title = lot.Title,
                        LotId = lot.Id,
                        Height = 136,
                        Description = String.Format("Стартовая цена: {0:0.00} BYN", lot.StartBid)
                    };
                    var content = lot.LotContents.FirstOrDefault();

                    if (content == null)
                        card.ImageSource = new BitmapImage(new Uri("../Images/empty_auction.jpg", UriKind.Relative));
                    else
                    {
                        card.ImageSource = ToImage(content.Content);
                    }

                    card.MouseDown += CardOnMouseDown;
                    lotStackPanel.Children.Add(card);
                });
            }
        }

        private void CardOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var card = (LotCard) sender;

            var window = (MainWindow)Application.Current.MainWindow;
            window.CurrentLot.ChangeLot(card.LotId);
        }


        private void Loaded_Cards(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) //не отображаем в конструкторе mainwindow
                return;

            Refresh();
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
