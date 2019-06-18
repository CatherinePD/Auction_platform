using System.Windows.Controls;
using System.Windows.Media;

namespace Auction.Controls
{
    /// <summary>
    /// Interaction logic for LotCard.xaml
    /// </summary>
    public partial class LotCard : UserControl
    {
        
        public LotCard()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => _title.Text;
            set => _title.Text = value;
        }

        public string Description
        {
            get => _desc.Text;
            set => _desc.Text = value;
        }

        public ImageSource ImageSource
        {
            get => _image.Source;
            set => _image.Source = value;
        }
        

        public int LotId { get; set; }

    }
}
