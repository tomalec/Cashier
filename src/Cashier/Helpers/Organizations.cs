using Simplified.Ring1;
using Simplified.Ring2;
using Simplified.Ring3;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.Helpers {
    public static class Organizations {
        public static Organization GetMyOrganization() {
            var settings = Db.SQL<Settings>("select s from Cashier.Settings s").First();
            return settings.OwnOrganization;
        }

        public static IEnumerable<Organization> GetMyShops() {
            return Db.SQL<Organization>("select o from Simplified.Ring2.Organization o WHERE Parent = ?", GetMyOrganization());
        }

        public static Organization GetSupplier() {
            var organization = new Organization();
            organization.Name = $"Generated supplier {organization.Key}";
            return organization;
        }

        public static Organization GetCustomer() {
            var organization = new Organization();
            organization.Name = $"Generated Customer {organization.Key}";
            return organization;
        }

        public static IEnumerable<Product> GetAvailableProducts(this Organization Shop) {
            return Db.SQL<Product>("SELECT P FROM Simplified.Ring3.Product P").Where(val => val.GetStockInShop(Shop) > 0);
        }

        /// <summary>
        /// Get current stock of given product in a given organization
        /// </summary>
        /// <param name="Shop">Stock will be calculated only for this organization</param>
        /// <param name="Product">Stock will be calculated for this product only</param>
        public static decimal GetStockInShop(this Organization Shop, Product Product) {            
            return Db.SQL<decimal>(
                    "SELECT SUM(PI.Quantity) FROM Simplified.Ring3.ProductInstance PI WHERE PI.IsAggregate = ? AND PI.Owner = ? and PI.Product = ?",
                    false, Shop, Product).First;
        }

        public static Transfer Sell(this Organization Shop, Product Product, decimal Quantity, DateTime? SellDate = null) {
            Organization customer = GetCustomer();
            ProductInstance many = Db.SQL<ProductInstance>("SELECT p FROM Simplified.Ring3.ProductInstance p WHERE p.Product = ? AND p.Owner = ? AND p.IsAggregate = ? AND p.Quantity > ?", Product, Shop, false, 0).First;
            if (many == null || many.Quantity < Quantity)
                throw new ArgumentOutOfRangeException("Quantity", "No ProductInstances to sell " + Quantity);

            var toSell = new ProductInstance() { Quantity = Quantity, Product = Product, Owner = Shop };
            if (many.Quantity > toSell.Quantity) {
                var notSold = new ProductInstance() { Quantity = many.Quantity - toSell.Quantity, Product = Product, Owner = Shop };
                new Subset() { WhatIs = notSold, ToWhat = many };
            }

            new Subset() { WhatIs = toSell, ToWhat = many };
            many.IsAggregate = true;

            DateTime date = SellDate ?? DateTime.Now;
            Transfer t = new Transfer() { When = date, ProductInstance = toSell, From = Shop, To = customer };
            toSell.Owner = customer;

            return t;
        }
    }
}
