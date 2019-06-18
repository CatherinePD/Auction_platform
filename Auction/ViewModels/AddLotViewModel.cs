using System;
using Auction.Common;
using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction.ViewModels
{
    public class AddLotViewModel: LotFormViewModel
    {
        public AddLotViewModel()
        {
            Action = FormMode.Add;
        }
        protected override void HandleLotAction(LotEntity lot)
        {
            lot.StartBid = decimal.Parse(StartBid);
            lot.CurrentBid = lot.StartBid;

            lot.DateCreated = DateTime.Now;
            lot.DateToExpire = DateTime.Now.AddDays(SelectedDaysCount.DaysCount);
            lot.OwnerId = Session.User.Id;
            lot.IsActive = true;

            LotService.AddLot(lot);
            View.OnLotAction(lot, FormMode.Add);
        }

        protected override void ClearForm(object obj)
        {
            Title = string.Empty;
            SelectedCategory = null;
            SelectedDaysCount = null;
            Photos.Clear();
            Description = string.Empty;
            StartBid = string.Empty;
        }
    }
}