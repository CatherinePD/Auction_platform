using System.Windows;
using System.Windows.Input;
using Auction.ViewModels;

namespace Auction
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>

    public interface IPasswordView
    {
        string OldPassword { get; set; }
        string NewPassword { get; set; }
        string NewPasswordConfirm { get; set; }
    }

    public partial class UserWindow : Window, IPasswordView
    {
        public UserWindow()
        {
            InitializeComponent();
            DataContext = new UserProfileViewModel(this);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string OldPassword
        {
            get => PassBoxOld.Password;
            set => PassBoxOld.Password = value;
        }
        public string NewPassword
        {
            get => PassBoxNew.Password;
            set => PassBoxNew.Password = value;
        }
        public string NewPasswordConfirm
        {
            get => PassBoxConfirm.Password;
            set => PassBoxConfirm.Password = value;
        }
    }
}
