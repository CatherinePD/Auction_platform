using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Auction.Common;
using Auction.DataAccess.Entities;
using Auction.Services;
using MaterialDesignThemes.Wpf;

namespace Auction.ViewModels
{
    public class UserProfileViewModel : ViewModelBase
    {
        private readonly Session _session;
        private readonly UserService _userService;
        private readonly IPasswordView _passwordView;
        private User _user;
        private FormMode _contactFormMode = FormMode.None;

        private string _login;
        private string _phone;
        private string _email;
        private string _address;
        private DateTime? _dateOfBirth;
        private byte[] _photo;
        private Visibility _defaultPhotoVisibility;
        private Visibility _userPhotoVisibility;

        public UserProfileViewModel(IPasswordView passwordView = null)
        {
            _passwordView = passwordView;
            _session = Session.CurrentSession;
            _userService = new UserService();

            _defaultPhotoVisibility = Visibility.Visible;
            _userPhotoVisibility = Visibility.Collapsed;

            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
            ConfirmCommand = new FormCommand(Confirm);
            CancelCommand = new FormCommand(Cancel);
            EditPhotoCommand = new FormCommand(EditPhoto);

            Initialize();
        }

        private void Initialize()
        {
            _user = _userService.GetUser(u => u.Id == _session.User.Id);
            Login = _user.Login;

            var contact = _user.Contact;
            if (contact != null)
            {
                _contactFormMode = FormMode.Update;

                Phone = contact.Phone;
                Email = contact.Email;
                Address = contact.Address;
                DateOfBirth = contact.DateOfBirth;
                Photo = contact.Photo;
            }
            else
            {
                _contactFormMode = FormMode.Add;
            }
        }

        #region properties
        public SnackbarMessageQueue MessageQueue { get; set; }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged(nameof(Login));
            }
        }
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        public byte[] Photo
        {
            get => _photo;
            set
            {
                if (value == null)
                {
                    DefaultPhotoVisibility = Visibility.Visible;
                    UserPhotoVisibility = Visibility.Collapsed;
                }
                else
                {
                    DefaultPhotoVisibility = Visibility.Collapsed;
                    UserPhotoVisibility = Visibility.Visible;
                }
                _photo = value;
                OnPropertyChanged(nameof(Photo));
            }
        }

        public Visibility DefaultPhotoVisibility
        {
            get => _defaultPhotoVisibility;
            set
            {
                _defaultPhotoVisibility = value;
                OnPropertyChanged(nameof(DefaultPhotoVisibility));
            }
        }

        public Visibility UserPhotoVisibility
        {
            get => _userPhotoVisibility;
            set
            {
                _userPhotoVisibility = value;
                OnPropertyChanged(nameof(UserPhotoVisibility));
            }
        }

        public string OldPassword
        {
            get => _passwordView.OldPassword;
            set => _passwordView.OldPassword = value;
        }
        public string NewPassword
        {
            get => _passwordView.NewPassword;
            set => _passwordView.NewPassword = value;
        }
        public string NewPasswordConfirm
        {
            get => _passwordView.NewPasswordConfirm;
            set => _passwordView.NewPasswordConfirm = value;
        } 

        public FormCommand ConfirmCommand { get; set; }

        public FormCommand CancelCommand { get; set; }

        public FormCommand EditPhotoCommand { get; set; }

        public bool IsPasswordsNotEmpty => !string.IsNullOrWhiteSpace(OldPassword) ||
                                         !string.IsNullOrWhiteSpace(NewPasswordConfirm) ||
                                         !string.IsNullOrWhiteSpace(NewPassword);

        public bool IsContactNotEmpty => !string.IsNullOrWhiteSpace(Address) ||
                                        !string.IsNullOrWhiteSpace(Email) ||
                                        !string.IsNullOrWhiteSpace(Phone) ||
                                        DateOfBirth != null ||
                                        Photo != null;
        #endregion

        private void Cancel(object param)
        {
            Initialize();
            ClearPasswordForm();
            ClearContactForm();
        }

        private void EditPhoto(object param)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openFileDlg.ShowDialog() == true)
            {
                string filename = openFileDlg.FileName;
                Photo = File.ReadAllBytes(filename);
            }
        }

        private void Confirm(object param)
        {
            ChangePasswordAndLogin();
            ChangeContact();

            var updatedUser = _userService.GetUser(u => u.Id == _session.User.Id);
            Session.SetCurrentUser(updatedUser, true);
        }

        private void ChangePasswordAndLogin()
        {
            if (IsPasswordsNotEmpty && IsValidPasswords())
            {
                _userService.ChangePassword(_user.Id, NewPassword);
                _user.Password = NewPassword;
                MessageQueue.Enqueue("Пароль изменен");
                ClearPasswordForm();
            }
            if (string.IsNullOrWhiteSpace(Login))
            {
                ShowError("Введите Логин", "Логин");
                return;
            }
            if (Login != _session.User.Login)
            {
                if (_userService.ChangeLogin(_user.Id, Login))
                {
                    _user.Login = Login;
                    MessageQueue.Enqueue("Логин изменен");
                }
                else
                    Login = _user.Login;
            }
        }

        private void ChangeContact()
        {
            OnPropertyChanged(nameof(Email));
            if (!IsValidContact())
            {
                ShowError("Введите Email", "Email");
                return;
            }

            if (_contactFormMode == FormMode.Add && IsContactNotEmpty)
            {
                _user.Contact = new Contact();
                FillContact(_user.Contact);
                _userService.AddContact(_user.Contact, _user.Id);
                MessageQueue.Enqueue("Контактная инфомация обновлена");
                return;
            }
            if (_contactFormMode == FormMode.Update && IsContactUpdateRequired())
            {
                FillContact(_user.Contact);
                _userService.UpdateContact(_user.Contact);
                MessageQueue.Enqueue("Контактная инфомация обновлена");
            }
        }

        private void FillContact(Contact contact)
        {
            contact.Address = Address;
            contact.Email = Email;
            contact.Phone = Phone;
            contact.DateOfBirth = DateOfBirth;
            contact.Photo = Photo;
        }

        private void ClearPasswordForm()
        {
            OldPassword = null;
            NewPassword = null;
            NewPasswordConfirm = null;
        }
        private void ClearContactForm()
        {
            if (_user.Contact == null)
            {
                Email = null;
                Phone = null;
                Photo = null;
                Address = null;
                DateOfBirth = null;
            }
        }

        private bool IsValidPasswords()
        {
            if (string.IsNullOrWhiteSpace(OldPassword))
            {
                ShowError("Введите старый пароль", "Пароль");
                return false;
            }
            if (NewPassword != NewPasswordConfirm || string.IsNullOrWhiteSpace(NewPassword) && string.IsNullOrWhiteSpace(NewPasswordConfirm))
            {
                ShowError("Корректно подтвердите новый пароль", "Пароль");
                return false;
            }
            if (OldPassword != _user.Password)
            {
                ShowError("Неверный пароль", "Пароль");
                return false;
            }

            return true;
        }

        private bool IsValidContact()
        {
            if (!IsContactNotEmpty && _user.Contact == null)
                return true;

            return !string.IsNullOrWhiteSpace(Email);
        }

        private bool IsContactUpdateRequired()
        {
            if (_user.Contact != null)
            {
                return Email != _user.Contact.Email ||
                       Phone != _user.Contact.Phone ||
                       Address != _user.Contact.Address ||
                       DateOfBirth != _user.Contact.DateOfBirth ||
                       Photo != _user.Contact.Photo;
            }
            return false;
        }


        public override string this[string columnName]//реализация IDataErrorInfo
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Email):
                        if (!IsValidContact())
                            return "Обязательное поле";
                        break;

                    case nameof(Login):
                        if (string.IsNullOrWhiteSpace(Login))
                            return "Обязательное поле";
                        break;
                }
                return string.Empty;
            }
        }

        private void ShowError(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}