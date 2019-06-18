using System.Collections.Generic;
using System.Windows;
using Auction.Common;
using Auction.Controls;
using Auction.DataAccess.Entities;
using Auction.Services;
using MaterialDesignThemes.Wpf;

namespace Auction.ViewModels
{
    public class CategoryListViewModel : ViewModelBase
    {
        private CategoryService _categoryService;

        public CategoryListViewModel()
        {
            _categoryService = new CategoryService();
            CategoryItems = _categoryService.GetCategories();

            LinkCommand = new FormCommand(OpenLink);
        }

        private void OpenLink(object obj)
        {
            if (obj is Category cat)
            {
                var window = (MainWindow)Application.Current.MainWindow;
                window.LotListControl.Refresh(l => l.IsActive && l.CategoryId == cat.Id, cat.Name);

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
        }

        public FormCommand LinkCommand { get; set; }
        public IList<Category> CategoryItems { get; set; }
    }
}