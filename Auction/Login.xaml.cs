using System.Windows;
using System.Windows.Input;
using Auction.Common;
using Auction.DataAccess.Entities;
using Auction.Services;

namespace Auction
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly UserService _userService;

        public Login()
        {
            InitializeComponent();
            _userService = new UserService();
        }
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var registration = new Registration();
            registration.Show();
            this.Close();
        }
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            if (_userService.TryAuthenticate(textBoxUserName.Text, textBoxPassword.Password, out User user))
            {
                Session.SetCurrentUser(user);
                this.Close();
            }
            else
            {
                MessageBox.Show("Логин или пароль введены неверно!", "Логин", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
