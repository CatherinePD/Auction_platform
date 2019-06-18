using System.Windows;
using Auction.Services;

namespace Auction
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            new LotService().RunLotExpirationProcessor();
        }
    }
}
