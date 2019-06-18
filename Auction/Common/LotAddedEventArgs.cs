using LotEntity = Auction.DataAccess.Entities.Lot;

namespace Auction.Common
{
    public class LotActionEventArgs
    {
        public LotActionEventArgs(LotEntity lot = null, FormMode action = FormMode.None)
        {
            Lot = lot;
            Action = action;
        }
        public LotEntity Lot { get; set; }
        public FormMode Action { get; set; }

    }
}