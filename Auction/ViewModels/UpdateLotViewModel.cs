using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Auction.Common;
using Auction.DataAccess.Entities;
using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction.ViewModels
{
    public class UpdateLotViewModel : LotFormViewModel
    {
        private LotEntity _lot;
        private Category _initialCategory;

        public UpdateLotViewModel()
        {
            Caption = "Обновление лота";
            Action = FormMode.Update;

            ValidatableFieldsNames = new List<string>
            {
                nameof(Title), nameof(SelectedCategory),
                nameof(Description), nameof(StartBid)
            };
        }

        /// <summary>
        /// заполнение формы редактирования лота
        /// </summary>
        /// <param name="lot"></param>
        public override void LoadLot(LotEntity lot)
        {
            _lot = lot;
            _initialCategory = Categories.FirstOrDefault(c => c.Id == _lot.CategoryId);

            LotId = _lot.Id;
            Title = _lot.Title;
            SelectedCategory = _initialCategory;
            Photos = new ObservableCollection<LotContent>(_lot.LotContents);
            Description = _lot.Description;
            StartBid = _lot.StartBid.ToString("0.00");

            int daysCount = (int) (_lot.DateToExpire - _lot.DateCreated).TotalDays;

            SelectedDaysCount = DaysCount.FirstOrDefault(l => l.DaysCount == daysCount);
        }

        protected override void HandleLotAction(LotEntity lot)
        {
            LotService.UpdateLot(lot, HasContentChanged);

            View.OnLotAction(lot, FormMode.Update);
        }

        protected override void ClearForm(object obj)
        {
            Title = _lot.Title;
            SelectedCategory = _initialCategory;
            Photos = new ObservableCollection<LotContent>(_lot.LotContents);
            Description = _lot.Description;
        }
    }
}