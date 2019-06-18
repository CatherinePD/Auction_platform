using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Auction.Common;
using Auction.Controls;
using Auction.DataAccess.Entities;
using Auction.Services;
using MaterialDesignThemes.Wpf;
using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction.ViewModels
{
    public class DisplayLotViewModel : ViewModelBase
    {
        private LotEntity _lot = new LotEntity();
        private User _owner = new User { Contact = new Contact() };
        private readonly LotService _lotService;
        private readonly Session _session;
        

        private string _title;
        private string _description;
        private string _categoryName;
        private int _daysToExpire;
        private DateTime _expireDate;
        private string _bid;
        private decimal _currentBid;
        private bool _isOwner;
        private bool _isExpired;
        private LotContent _selectedContent;
        private int _bidsCount;
        private ObservableCollection<LotContent> _content;
        private ObservableCollection<Bid> _bids;

        public DisplayLotViewModel()
        {
            _session = Session.CurrentSession;
            _session.CurrentUserUpdated += SessionOnCurrentUserUpdated;

            MakeBidCommand = new FormCommand(MakeBid, CanExecuteMakeBid);
            CloseLotCommand = new FormCommand(CloseLot);
            EditLotCommand = new FormCommand(EditLot);
            OpenBidHistoryCommand = new FormCommand(OpenBidHistory, OpenBidHistoryCanExecute);
            ShowSellerLotsCommand = new FormCommand(ShowSellerLots);

            _lotService = new LotService();

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                IsLotLoaded = true;
        }

        private void ShowSellerLots(object obj)
        {
            if (_owner == null)
                return;
            var window = (MainWindow)Application.Current.MainWindow;
            window.LotListControl.Refresh(l => l.IsActive && l.OwnerId == _owner.Id, $"Лоты продавца {_owner.Login}");
        }

        private bool OpenBidHistoryCanExecute(object arg)
        {
            return _bids != null && _bids.Any();
        }

        private async void OpenBidHistory(object obj)
        {
            var view = new BidHistory
            {
                DataContext = new BidHistoryViewModel
                {
                    Bids = new ObservableCollection<Bid>(_bids.OrderByDescending(b => b.Amount)),
                    Title = _title
                }
            };

            await DialogHost.Show(view, "RootDialog");
        }


        private void EditLot(object obj)
        {
            View.OnLotEdit(_lot);
        }

        public void Initialize(int lotId)
        {
            _lot = _lotService.GetLot(lotId);

            if (_lot != null)
            {
                _owner = _lot.Owner;
                Title = _lot.Title;
                Description = _lot.Description;
                CategoryName = _lot.Category.Name;
                CurrentBid = _lot.CurrentBid;
                ExpireDate = _lot.DateToExpire;
                BidsCount = _lot.Bids.Count;
                _bids = new ObservableCollection<Bid>(_lot.Bids.ToList());

                LoadContent(_lot);

                double daysToExpire = (_lot.DateToExpire - DateTime.Now).TotalDays;

                if (daysToExpire < 0)
                {
                    _isExpired = true;
                    NotifyExpired();
                    DaysToExpire = 0;
                }
                else
                    DaysToExpire = (int)Math.Ceiling(daysToExpire);

                if (_session.IsLoggedIn)
                    CurrentUserIsOwner = _owner.Id == _session.User.Id;

                IsLotLoaded = true;

                if (!IsActive)
                {
                    DaysToExpire = 0;
                    Title += " (не активен)";
                }
            }

            OnPropertyChanged(nameof(IsLotLoaded));
            OnPropertyChanged(nameof(LotIsNotLoaded));
        }

        #region properties
        public FormCommand MakeBidCommand { get; set; }
        public FormCommand CloseLotCommand { get; set; }
        public FormCommand EditLotCommand { get; set; }
        public FormCommand OpenBidHistoryCommand { get; set; }
        public FormCommand ShowSellerLotsCommand { get; set; }
        public IDisplayLotView View { get; set; }
        public bool OwnerHasUserPic => _owner.Contact?.Photo != null;
        public bool OwnerHasNoUserPic => !OwnerHasUserPic;
        public bool IsLotLoaded { get; private set; }
        public bool LotIsNotLoaded => !IsLotLoaded;

        public bool IsContentEmpty
        {
            get
            {
                if (LotContents == null)
                    return true;
                return !LotContents.Any();
            }
        }

        public ObservableCollection<LotContent> LotContents
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(LotContents));
                OnPropertyChanged(nameof(IsContentEmpty));
            }
        }

        public LotContent SelectedContent
        {
            get => _selectedContent;
            set
            {
                _selectedContent = value;
                OnPropertyChanged(nameof(SelectedContent));
            }
        }

        public decimal CurrentBid
        {
            get => _currentBid;
            set
            {
                _currentBid = value;
                OnPropertyChanged(nameof(CurrentBid));
            }
        }

        public bool CurrentUserIsOwner
        {
            get => _isOwner;
            set
            {
                _isOwner = value;
                OnPropertyChanged(nameof(CurrentUserIsOwner));
                OnPropertyChanged(nameof(CurrentUserIsNotOwner));
            }
        }

        public bool CurrentUserIsNotOwner => !CurrentUserIsOwner;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }

        public int DaysToExpire
        {
            get => _daysToExpire;
            set
            {
                _daysToExpire = value;
                OnPropertyChanged(nameof(DaysToExpire));
            }
        }

        public DateTime ExpireDate
        {
            get => _expireDate;
            set
            {
                _expireDate = value;
                OnPropertyChanged(nameof(ExpireDate));
                OnPropertyChanged(nameof(ExpireDateMessage));
            }
        }

        public string ExpireDateMessage
        {
            get
            {
                if (IsActive)
                    return ExpireDate.ToString("dd.MM.yyyy HH:mm");

                return "Завершен";
            }
        }

        public string Bid
        {
            get => _bid;
            set
            {
                _bid = value;
                OnPropertyChanged(nameof(Bid));
            }
        }

        public string OwnerLogin
        {
            get => _owner.Login;
            set
            {
                _owner.Login = value;
                OnPropertyChanged(nameof(OwnerLogin));
            }
        }

        public byte[] OwnerPhoto
        {
            get => _owner.Contact?.Photo;
            set
            {
                _owner.Contact.Photo = value;
                OnPropertyChanged(nameof(OwnerPhoto));
                OnPropertyChanged(nameof(OwnerHasUserPic));
                OnPropertyChanged(nameof(OwnerHasNoUserPic));
            }
        }

        public string OwnerPhone
        {
            get
            {if (_owner.Contact?.Phone != null)
                    return _owner.Contact.Phone;
                else
                    return "Данные отсутствуют";
            }
            set
            {
                _owner.Contact.Phone = value;
                OnPropertyChanged(nameof(OwnerPhone));
            }
        }
        public string OwnerEmail
        {
            get
            {
                if (_owner.Contact?.Email != null)
                    return _owner.Contact.Email;
                else
                    return "Данные отсутствуют";
            }
            set
            {
                _owner.Contact.Email = value;
                OnPropertyChanged(nameof(OwnerEmail));
            }
        }

        public int BidsCount
        {
            get =>  _bidsCount;
            set
            {
                _bidsCount = value;
                OnPropertyChanged(nameof(BidsCount));
            }
        }

        public bool IsActive => _lot != null && _lot.IsActive;

        #endregion

        public override string this[string columnName]
        {
            get
            {
                if (View == null)
                    return string.Empty;

                if (columnName == nameof(Bid))
                {
                    if (string.IsNullOrWhiteSpace(Bid))
                        return string.Empty;
                    if (!decimal.TryParse(Bid, out var result))
                        return "Введите корректную цену";
                    if (result > 10_000_000_000)
                        return "Превышена допустимая цена";
                }
                return string.Empty;
            }
        }

        private void LoadContent(LotEntity lot)
        {
            var content = lot.LotContents.ToList();
            LotContents = new ObservableCollection<LotContent>(content);
            SelectedContent = LotContents.FirstOrDefault();
        }

        private void CloseLot(object obj)
        {
            _lotService.DisableLot(_lot.Id);
            _lot.IsActive = false;

            DaysToExpire = 0;
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(ExpireDateMessage));
            Title += " (не активен)";
        }

        private void NotifyExpired()
        {
            if (_lot.IsActive && _isExpired)
            {
                CloseLot(null);
            }
        }

        private bool CanExecuteMakeBid(object arg)
        {
            return !_isExpired &&
                    !string.IsNullOrWhiteSpace(Bid) &&
                    string.IsNullOrWhiteSpace(this[nameof(Bid)]);
        }

        private void MakeBid(object obj)
        {
            if (!_session.IsLoggedIn)
            {
                MessageBox.Show("Чтобы сделать ставку, зайдите в свою учётную запись!", "Ставка", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            if (decimal.Parse(Bid) <= CurrentBid)
            {
                MessageBox.Show("Ставка должна быть выше текущей!", "Ставка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var bid = new Bid
            {
                Amount = decimal.Parse(Bid),
                UserId = _session.User.Id,
                DateCreated = DateTime.Now,
                LotId = _lot.Id
            };

            _lotService.AddBid(bid);
            CurrentBid = bid.Amount;
            _bids.Add(bid);
            BidsCount++;
        }

        private void SessionOnCurrentUserUpdated(object sender, SessionEventArgs sessionEventArgs)
        {
            if (_session.IsLoggedIn)
                CurrentUserIsOwner = _owner.Id == _session.User.Id;
        }
    }
}