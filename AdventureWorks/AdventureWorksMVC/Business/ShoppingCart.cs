using System.Collections.Generic;
using AdventureWorksDataModel;

namespace EpicAdventureWorks
{
    /// <summary>
    /// Summary description for ShoppingCart
    /// </summary>
    public class ShoppingCart
    {
        private readonly Contact _contact;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCart"/> class.
        /// </summary>
        /// <param name="c">The c.</param>
        public ShoppingCart(Contact c)
        {
            _contact = c;
        }

        /// <summary>
        /// Gets or sets the shopping cart ID.
        /// </summary>
        public int ShoppingCartId
        {
            get
            {
                if (_contact == null)
                {
                    return -1;
                }
                return _contact.ContactID;
            }
        }

        /// <summary>
        /// Gets or sets the shopping car items.
        /// </summary>
        public List<ShoppingCartItem> ShoppingCarItems
        {
            get
            {
                List<ShoppingCartItem> ls = ShoppingCartManager.GetShoppingCartItems(ShoppingCartId);
                foreach (ShoppingCartItem item in ls)
                {
                    if (item.Product == null)
                    {
                        item.ProductReference.Load();
                    }
                }
                return ls;
            }
        }

    }
}