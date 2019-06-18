using System;
using System.Windows;
using System.Windows.Input;
using Auction.Common;
using Auction.ViewModels;
using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction
{
    /// <summary>
    /// Логика взаимодействия для AddLotWindow.xaml
    /// </summary>
    public interface IAddEditView
    {
        void OnLotAction(LotEntity lot, FormMode action);
    }

    public partial class AddLotWindow : Window, IAddEditView
    {
        /// <summary>
        /// Событие возникает при добавлении либо редактировании лота
        /// </summary>
        public event EventHandler<LotActionEventArgs> LotAction;

        public AddLotWindow(FormMode action)
        {
            InitializeComponent();

            this.Loaded += OnLoaded;

            if (action == FormMode.Add)
                DataContext = new AddLotViewModel();
            else if (action == FormMode.Update)
                DataContext = new UpdateLotViewModel();

            
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ((LotFormViewModel)DataContext).View = this;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        public virtual void OnLotAction(LotEntity lot, FormMode action)
        {
            LotAction?.Invoke(this, new LotActionEventArgs(lot, action));
            this.Close();
        }
    }
}
