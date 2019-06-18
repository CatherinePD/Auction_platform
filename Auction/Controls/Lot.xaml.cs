using System.Windows;
using System.Windows.Controls;
using Auction.Common;
using Auction.ViewModels;

namespace Auction.Controls
{
    public interface IDisplayLotView
    {
        void OnLotEdit(DataAccess.Entities.Lot lot);
    }

    /// <summary>
    /// Логика взаимодействия для Lot.xaml
    /// </summary>
    public partial class Lot : UserControl, IDisplayLotView
    {
        public Lot()
        {
            InitializeComponent();

            var viewModel = (DisplayLotViewModel) DataContext;
            viewModel.View = this;
        }

        public void ChangeLot(int newLotId)//клик по карточке
        {
            var viewModel = new DisplayLotViewModel();
            viewModel.Initialize(newLotId);
            DataContext = viewModel;
            viewModel.View = this;
        }

        public void OnLotEdit(DataAccess.Entities.Lot lot)//клик на кнопку редактирования лота
        {
            var editLotWindow = new AddLotWindow(FormMode.Update);
            editLotWindow.LotAction += OnLotUpdated;
            ((UpdateLotViewModel)editLotWindow.DataContext).LoadLot(lot);

            editLotWindow.ShowDialog();
        }

        private void OnLotUpdated(object sender, LotActionEventArgs e)
        {
            ChangeLot(e.Lot.Id);

            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow?.LotListControl.Refresh();
        }
    }
}
