using Simplified.Ring2;
using Simplified.Ring3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashier.Helpers {
    public static class Products {

        /// <summary>
        /// Get current stock of given product in a given organization
        /// </summary>
        /// <param name="Product">Stock will be calculated for this product only</param>
        /// <param name="Shop">Stock will be calculated only for this organization</param>
        public static decimal GetStockInShop(this Product Product, Organization Shop) {
            return Shop.GetStockInShop(Product);
        }
    }
}
