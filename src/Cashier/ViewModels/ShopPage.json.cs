using System;
using Simplified.Ring2;
using Starcounter;
using Cashier.Helpers;
using Simplified.Ring3;
using System.Globalization;

namespace Cashier {
    partial class ShopPage : Page, IBound<Organization> {
        public DateTime? SellDateValue
        {
            get
            {
                DateTime date;
                if (DateTime.TryParseExact(SellDate, ServerDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) {
                    date = date + DateTime.Now.TimeOfDay;
                } else {
                    date = DateTime.Now;
                }
                return date;
            }
        }

        protected override void OnData() {
            base.OnData();
            ShopKey = Data.Key;
            CurrentShop.Data = Data;
            SellDate = DateTime.Now.ToString(ServerDateFormat);
            Shops.Data = Helpers.Organizations.GetMyShops();
            Products.Data = Data.GetAvailableProducts();
        }

        private void Handle(Input.ShopKey action) {
            if (string.IsNullOrEmpty(action.Value))
                return;
            RedirectUrl = "/cashier/shop/" + action.Value;
        }

        private void Sell(ProductsItem product) {
            Data.Sell(product.Data, product.Quantity > 1 ? 1 : product.Quantity, SellDateValue);
        }

        [ShopPage_json.CurrentShop]
        public partial class ShopPageCurrentShop : Page, IBound<Organization> {
        }

        [ShopPage_json.Shops]
        public partial class ShopsItem : Page, IBound<Organization> {
        }

        [ShopPage_json.Products]
        public partial class ProductsItem : Page, IBound<Product> {
            public ShopPage ParentPage
            {
                get
                {
                    return this.Parent.Parent as ShopPage;
                }
            }

            protected override void OnData() {
                base.OnData();
                Quantity = this.Data.GetStockInShop(ParentPage.Data);
            }

            private void Handle(Input.SellClick action) {
                Quantity = this.Data.GetStockInShop(ParentPage.Data);
                if (Quantity > 0) {
                    ParentPage.Sell(this);
                    Transaction.Commit();
                }
                Quantity = this.Data.GetStockInShop(ParentPage.Data);
            }
        }
    }
}
