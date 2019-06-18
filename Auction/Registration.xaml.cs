using System.Windows;
using System.Windows.Input;
using Auction.DataAccess.Entities;
using Auction.Services;

namespace Auction
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private readonly UserService _userService;

        public Registration()
        {
            InitializeComponent();

            _userService = new UserService();
        }
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                Login = textBoxUserName.Text,
                Password = textBoxPassword.Password
            };

            if (!_userService.TryRegistration(user, textConfirmBoxPassword.Password, out var message))
            {
                MessageBox.Show(message, "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var login = new Login();
            this.Close();
            login.ShowDialog();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
