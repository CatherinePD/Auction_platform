using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Auction.Common;
using Auction.DataAccess.Entities;
using Auction.Services;
using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction.ViewModels
{
    public abstract class LotFormViewModel : ViewModelBase
    {
        private const int MAX_PHOTOS = 5;
        protected readonly CategoryService CategoryService;
        protected readonly LotService LotService;
        protected readonly Session Session;
        protected List<string> ValidatableFieldsNames;

        protected bool HasContentChanged;
        protected int LotId;
        private FormMode _action;
        private string _caption;
        private string _title;
        private Category _selectedCategory;
        private LotDaysToExpire _selectedDaysCount;
        private string _description;
        private string _startBid;
        private ObservableCollection<LotContent> _photos;

        protected LotFormViewModel()
        {
            LotService = new LotService();
            CategoryService = new CategoryService();
            Session = Session.CurrentSession;

            Categories = CategoryService.GetCategories();
            ConfirmCommand = new FormCommand(Confirm, ConfirmCanExecute);
            AddPhotoCommand = new FormCommand(AddPhoto, AddPhotoCanExecute);
            DeletePhotoCommand = new FormCommand(DeletePhoto);
            CancelCommand = new FormCommand(ClearForm);

            Photos = new ObservableCollection<LotContent>();

            DaysCount = new List<LotDaysToExpire>
            {
                new LotDaysToExpire(3, "3 дня"),
                new LotDaysToExpire(5, "5 дней"),
                new LotDaysToExpire(7, "7 дней"),
                new LotDaysToExpire(10, "10 дней"),
                new LotDaysToExpire(14, "14 дней"),
                new LotDaysToExpire(21, "21 день"),
                new LotDaysToExpire(30, "30 дней")
            };

            ValidatableFieldsNames = new List<string>
            {
                nameof(Title), nameof(SelectedCategory),
                nameof(Description), nameof(StartBid),
                nameof(SelectedDaysCount)
            };
        }

        #region properties
        public FormCommand ConfirmCommand { get; set; }
        public FormCommand AddPhotoCommand { get; set; }
        public FormCommand DeletePhotoCommand { get; set; }
        public FormCommand CancelCommand { get; set; }
        public IList<LotDaysToExpire> DaysCount { get; set; }
        public IList<Category> Categories { get; set; }
        public IAddEditView View { get; set; }

        public ObservableCollection<LotContent> Photos
        {
            get => _photos;
            set
            {
                _photos = value;
                OnPropertyChanged(nameof(Photos));
            }
        }

        protected FormMode Action
        {
            get => _action;
            set
            {
                _action = value;

                if (_action == FormMode.Update)
                    Caption = "Редактирование лота";
                if (_action == FormMode.Add)
                    Caption = "Создание лота";

                OnPropertyChanged(nameof(IsAddMode));
            }
        }

        public bool IsAddMode => _action == FormMode.Add;

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(_caption));
            }
        }

        public LotDaysToExpire SelectedDaysCount
        {
            get => _selectedDaysCount;
            set
            {
                _selectedDaysCount = value;
                OnPropertyChanged(nameof(SelectedDaysCount));
            }
        }
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

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

        public string StartBid
        {
            get => _startBid;
            set
            {
                _startBid = value;
                OnPropertyChanged(nameof(StartBid));
            }
        }
        #endregion

        public virtual void LoadLot(LotEntity lot)
        {
        }

        protected abstract void HandleLotAction(LotEntity lot);

        protected abstract void ClearForm(object obj);


        public override string this[string columnName]
        {
            get
            {
                if (View == null)
                    return string.Empty;

                switch (columnName)
                {
                    case nameof(Title):
                        if (string.IsNullOrWhiteSpace(Title))
                            return "Обязательное поле";
                        if (Title.Length > 75)
                            return "Превышено допустимое количесвто символов";
                        break;

                    case nameof(Description):
                        if (string.IsNullOrWhiteSpace(Description))
                            return "Обязательное поле";
                        break;

                    case nameof(SelectedCategory):
                        if (SelectedCategory == null)
                            return "Обязательное поле";
                        break;

                    case nameof(StartBid):
                        if (string.IsNullOrWhiteSpace(StartBid))
                            return "Обязательное поле";
                        if (!decimal.TryParse(StartBid, out var result))
                            return "Введите корректную цену";
                        if (result > 10_000_000_000)
                            return "Превышена допустимая цена";
                        break;

                    case nameof(SelectedDaysCount):
                        if (SelectedDaysCount == null)
                            return "Обязательное поле";
                        break;
                }
                return string.Empty;
            }
        }

        protected void Confirm(object o)
        {
            var lot = new LotEntity();

            lot.Id = LotId;
            lot.Title = Title;
            lot.Description = Description;
            lot.CategoryId = SelectedCategory.Id;
            lot.LotContents = Photos.ToList();

            HandleLotAction(lot);
        }

        protected bool ConfirmCanExecute(object arg)
        {
            return ValidateViewModel();
        }

        protected bool ValidateViewModel()
        {
            return ValidatableFieldsNames.All(name => string.IsNullOrWhiteSpace(this[name]));
        }

        private void DeletePhoto(object obj)
        {
            Photos.Remove((LotContent)obj);
            HasContentChanged = true;
        }

        private bool AddPhotoCanExecute(object o)
        {
            return Photos.Count < MAX_PHOTOS;
        }

        private void AddPhoto(object o)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openFileDlg.ShowDialog() == true)
            {
                string filename = openFileDlg.FileName;
                var bytes = File.ReadAllBytes(filename);
                Photos.Add(new LotContent { Content = bytes });
                HasContentChanged = true;
            }
        }
    }

    public class LotDaysToExpire
    {
        public LotDaysToExpire(int days = 0, string caption = null)
        {
            DaysCount = days;
            Caption = caption;
        }
        public string Caption { get; set; }
        public int DaysCount { get; set; }
    }
}