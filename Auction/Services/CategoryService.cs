using System.Collections.Generic;
using System.Linq;
using Auction.DataAccess.Entities;
using Auction.DataAccess.UnitOfWork;

namespace Auction.Services
{
    public class CategoryService
    {
        public List<Category> GetCategories()
        {
            using (var unitOfWork = new AuctionUnitOfWork())
            {
                return unitOfWork.CategoryRepository.Get().ToList();
            }
        }
    }
}