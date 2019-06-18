using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Auction.Common;
using Auction.Controls;
using Auction.DataAccess;
using Auction.Notification;
using Auction.Notification.EventsModels;
using Auction.Services;
using MaterialDesignThemes.Wpf;
using NotificationEntity = Auction.DataAccess.Entities.Notification;

namespace Auction
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Session _session;
        private readonly UserService _userService;
        private DataNotificator _notificator;
        private readonly NotificationService _notificationService;

        private ObservableCollection<NotificationEntity> _notifications;
        private int? _badgeCounter;
        private bool _isNewNotifiaction;

        public MainWindow()
        {
            InitializeComponent();
            _session = Session.CurrentSession;
            _userService = new UserService();
            _notificationService = new NotificationService();
            DataContext = this;
            OpenCategoriesCommand = new FormCommand(GetCategories);

            _session.CurrentUserUpdated += _session_CurrentUserUpdated;
            _session.CurrentUserUpdated += SubscribeOnLoginLogout;
            this.Closing += OnClosing;
        }

        public ObservableCollection<NotificationEntity> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged(nameof(Notifications));
            }
        }

        public int? BadgeCounter
        {
            get => _badgeCounter;
            set
            {
                _badgeCounter = value;
                OnPropertyChanged(nameof(BadgeCounter));
            }
        }

        public bool IsAnyNewNotifications
        {
            get => _isNewNotifiaction;
            set
            {
                _isNewNotifiaction = value;
                OnPropertyChanged(nameof(IsAnyNewNotifications));
            }
        }



        #region Notifications
        private async void SubscribeOnLoginLogout(object sender, SessionEventArgs e)
        {
            if (e.IsHandled) return;

            if (_session.IsLoggedIn) // on Login
            {
                LoadExistingNotifications();

                DataNotificator.Register(ConnectionConstants.ConnectionString,
                                         ConnectionConstants.DatabaseName,
                                         "Notifications", _session.User.Id);

                _notificator = DataNotificator.Instance;
                _notificator.OnDataUpdated += NotificatorOnDataUpdated;

                await _notificator.Listen();
            }
            else // on Logout
            {
                NotificationsMarkAsRead();

                Notifications = null;
                BadgeCounter = null;
                IsAnyNewNotifications = false;
                _notificator.OnDataUpdated -= NotificatorOnDataUpdated;
                await StopListenerAsync();
            }
        }

        private void LoadExistingNotifications()
        {
            var notifications = _notificationService.GetUserNotifications(_session.User.Id);
            Notifications = new ObservableCollection<NotificationEntity>(notifications);

            if (Notifications.Any())
            {
                BadgeCounter = Notifications.Count;
                IsAnyNewNotifications = true;
            }
        }

        private void NotificationsMarkAsRead()
        {
            var ids = Notifications.Where(n => n.IsReaded).Select(n => n.Id);

            if (ids.Any())
                _notificationService.MarkAsReaded(ids);
        }

        private void NotificatorOnDataUpdated(object sender, DataUpdatedEventArgs e)
        {
            if (!_session.IsLoggedIn)
                return;

            var notifiaction = e.GetNotificationObject<NotificationEntity>();

            foreach (var inserted in notifiaction.Inserted)
            {
                if (_session.User.Id != inserted.RecieverId) continue;

                this.Dispatcher.Invoke(() =>
                {
                    Notifications.Add(inserted);

                    if (BadgeCounter == null)
                        BadgeCounter = 0;

                    BadgeCounter++;
                    IsAnyNewNotifications = true;
                });
            }
        }

        private void OnClosing(object sender, EventArgs cancelEventArgs)
        {
            if (!_session.IsLoggedIn) return;

            StopListenerAsync().ConfigureAwait(false);
            NotificationsMarkAsRead();
        }

        private async Task StopListenerAsync()
        {
            await Task.Factory.StartNew(() => _notificator.Stop());
        }

        private void PopupBox_OnOpened(object sender, RoutedEventArgs e)
        {
            foreach (var notification in Notifications)
            {
                notification.IsReaded = true;
            }

            BadgeCounter = null;
        } 
        #endregion

        private async void GetCategories(object obj)
        {
            var view = new CategoryList();

            var result = await DialogHost.Show(view, "RootDialog");
        }

        public FormCommand OpenCategoriesCommand { get; set; }

        private void _session_CurrentUserUpdated(object sender, SessionEventArgs e)
        {
            if (_session.IsLoggedIn)
            {
                RegistrationEnterDrawerHostStackPanel.Visibility = Visibility.Collapsed;
                MyProfileExitDrawerHostStackPanel.Visibility = Visibility.Visible;
                NameOfAppTextBlock.Text = "в Sellaby,";
                NameOfUserTextBlock.Visibility = Visibility.Visible;
                NameOfUserTextBlock.Text = _session.User.Login;
            }
            else
            {
                RegistrationEnterDrawerHostStackPanel.Visibility = Visibility.Visible;
                MyProfileExitDrawerHostStackPanel.Visibility = Visibility.Collapsed;
                NameOfUserTextBlock.Visibility = Visibility.Collapsed;
                NameOfAppTextBlock.Text = "в Sellaby";
            }
        }

        private void ButtonRegistration_Click(object sender, RoutedEventArgs e)
        {
            var registration = new Registration();
            registration.ShowDialog();
        }
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.ShowDialog();
        }

        private void MyProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var userwindow = new UserWindow();
            userwindow.ShowDialog();

        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            _userService.LogOut();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var info = new Info();
            info.ShowDialog();
        }

        private void NewLotButton_Click(object sender, RoutedEventArgs e)
        {
            if(_session.IsLoggedIn)
            {
                var addLotWindow = new AddLotWindow(FormMode.Add);
                addLotWindow.LotAction += OnLotAdded;

                var oldBackground = this.Background;
                this.Background = Brushes.LightGray;
                this.Opacity = 0.5;

                addLotWindow.ShowDialog();

                this.Opacity = 1;
                this.Background = oldBackground;
            }
            else
            {
               var result = ShowError("Для создания лота необходимо войти в систему! Желаете войти?", "Создание лота");

               if (result == MessageBoxResult.Yes)
                    ButtonLogin_Click(sender,e);
            }
        }

        private void OnLotAdded(object sender, LotActionEventArgs lotAddedEventArgs)
        {
            LotListControl.Refresh();
        }

        private MessageBoxResult ShowError(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            var text = SearchTextBox.Text;
            LotListControl.Refresh(l => l.IsActive && l.Title.Contains(text), $"Поиск: {text}");
        }

        private void ResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            LotListControl.Refresh();
        }

        private void SellingButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_session.IsLoggedIn)
            {
                LotListControl.Refresh(l => l.OwnerId == _session.User.Id &&
                                            (l.IsActive == SellingActiveCheckBox.IsChecked ||
                                            l.IsActive == !SellingInactiveCheckBox.IsChecked), "Мои лоты");
            }
            else
            {
                var result = ShowError("Для поиска своих лотов необходимо войти в систему! Желаете войти?", "Поиск");

                if (result == MessageBoxResult.Yes)
                    ButtonLogin_Click(sender, e);
            }
        }

        private void MyBidsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_session.IsLoggedIn)
            {
                LotListControl.Refresh(l => l.Users.AsQueryable().Select(u => u.Id).Contains(_session.User.Id) &&
                                            (l.IsActive == BidActiveCheckBox.IsChecked ||
                                            l.IsActive == !BidInactiveCheckBox.IsChecked), "Мои ставки");
            }
            else
            {
                var result = ShowError("Для поиска своих лотов необходимо войти в систему! Желаете войти?", "Поиск");

                if (result == MessageBoxResult.Yes)
                    ButtonLogin_Click(sender, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PurchasesButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_session.IsLoggedIn)
            {
                LotListControl.Refresh(l =>!l.IsActive && 
                            l.Bids.OrderByDescending(b => b.Amount).FirstOrDefault().UserId == _session.User.Id, "Мои покупки");
            }
            else
            {
                var result = ShowError("Для поиска своих покупок необходимо войти в систему! Желаете войти?", "Поиск");

                if (result == MessageBoxResult.Yes)
                    ButtonLogin_Click(sender, e);
            }
        }
    }
}
