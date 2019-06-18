using System.Collections.Generic;
using System.Data.Entity;
using Auction.DataAccess.Entities;

namespace Auction.DataAccess
{
    public class DatabaseInitializer : CreateDatabaseIfNotExists<AuctionContext>
    {
        protected override void Seed(AuctionContext context)
        {
            var categoryList = new List<Category>
            {
                new Category {Name = "Автомобили, запчасти, GPS"},
                new Category {Name = "Детские товары, игрушки"},
                new Category {Name = "Дом, сад, интерьер"},
                new Category {Name = "Интернет, игры, софт"},
                new Category {Name = "Искусство, антиквариат"},
                new Category {Name = "Книги, журналы"},
                new Category {Name = "Коллекционирование"},
                new Category {Name = "Красота и здоровье"},
                new Category {Name = "Мобильные телефоны, смартфоны"},
                new Category {Name = "Монеты"},
                new Category {Name = "Музыка, инструменты"},
                new Category {Name = "Недвижимость"},
                new Category {Name = "Ноутбуки, компьютеры"},
                new Category {Name = "Подарки, сувениры, handmade"},
                new Category {Name = "Одежда, обувь, аксессуары"},
                new Category {Name = "Спорт, туризм, отдых"},
                new Category {Name = "Техника, оборудование"},
                new Category {Name = "Услуги, работа"},
                new Category {Name = "Фильмы"},
                new Category {Name = "Электроника"}
            };
            context.Categories.AddRange(categoryList);
            base.Seed(context);
        }
    }
}