using System.Collections.ObjectModel;
using Auction.DataAccess.Entities;

namespace Auction.ViewModels
{
    public class BidHistoryViewModel: ViewModelBase
    {
        private ObservableCollection<Bid> _bids;
        private string _title;

        public ObservableCollection<Bid> Bids
        {
            get => _bids;
            set
            {
                _bids = value;
                OnPropertyChanged(nameof(Bids));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(Title);
            }
        }
    }
}