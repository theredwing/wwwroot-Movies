using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdventureWorksDataModel;

namespace EpicAdventureWorks
{
    /// <summary>
    /// Contains methods with related to shopping cart. 
    /// </summary>
    public class ShoppingCartManager
    {

        /// <summary>
        /// Deletes the shopping cart item.
        /// </summary>
        /// <param name="shoppingCartItemID">The ID of shopping cart.</param>
        public static void DeleteShoppingCartItemByCartID(string shoppingCartID)
        {
            Entities entities = Common.DataEntities;
            var shopCartItems = from item in entities.ShoppingCartItem
                                where item.ShoppingCartID == shoppingCartID
                                select item;
            if (shopCartItems != null && shopCartItems.Count() > 0)
            {
                foreach (ShoppingCartItem shoppingCartItem in shopCartItems)
                {
                    entities.DeleteObject(shoppingCartItem);
                }
                entities.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the shopping cart item.
        /// </summary>
        /// <param name="shoppingCartItemID">The ID of shopping cart item.</param>
        public static void DeleteShoppingCartItem(int shoppingCartItemID)
        {
            if (RequestContext.Current.Contact == null)
            {
                List<ShoppingCartItem> ls = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
                var item = from l in ls
                                        where l.ShoppingCartItemID == shoppingCartItemID
                                        select l;
                ShoppingCartItem s = item.FirstOrDefault();
                if (s!=null)
                {
                    ls.Remove(s);
                    RequestContext.Current.SaveShoppingCartItemsToCookie(ls);
                }
            }
            else
            {
                Entities entities = Common.DataEntities;
                var shopCartItems = from item in entities.ShoppingCartItem
                                    where item.ShoppingCartItemID == shoppingCartItemID
                                    select item;
                if (shopCartItems != null && shopCartItems.Count() > 0)
                {
                    ShoppingCartItem shoppingCartItem = shopCartItems.FirstOrDefault();

                    entities.DeleteObject(shoppingCartItem);
                    entities.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Gets the shopping cart item by ID.
        /// </summary>
        /// <param name="id">The ID of shopping cart item.</param>
        /// <returns></returns>
        public static ShoppingCartItem GetShoppingCartItemByID(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            var shopCartItems = from item in Common.DataEntities.ShoppingCartItem
                                where item.ShoppingCartItemID == id
                                select new
                                {
                                    item,
                                    item.Product
                                };
            if (shopCartItems != null && shopCartItems.Count() > 0)
                return shopCartItems.FirstOrDefault().item;
            else
                return null;
        }

        /// <summary>
        /// Adds product to cart by giving a product ID.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="quantity">The quantity of the  product.</param>
        /// <returns></returns>
        public static void AddToCart(int productId, int quantity)
        {
            if (productId<=0 || quantity<=0)
            {
                return;
            }
            Entities e = Common.DataEntities;
            Product p = ProductManager.GetProductByProductId(productId, e);
            if (p == null)
            {
                return;
            }
            if (RequestContext.Current.Contact == null)
            {
                HttpCookie cols = RequestContext.Current.CartItemsFromCookie;
                List<ShoppingCartItem> ls = new List<ShoppingCartItem>();
                if (cols != null && cols.Values.Count > 0)
                {
                    string[] pairs = cols.Values[0].Split(',');
                    for (int i = 0; i < pairs.Length; i++)
                    {
                        string value = pairs[i];
                        if (string.IsNullOrEmpty(value))
                        {
                            continue;
                        }
                        string[] qs = value.Split('_');
                        int iq = Int32.Parse(qs[0]);
                        int ipid = Int32.Parse(qs[1]);
                        if (iq <= 0 || ipid <= 0)
                        {
                            continue;
                        }
                        Product client = ProductManager.GetProductByProductId(ipid, e);
                        if (client == null)
                        {
                            continue;
                        }
                        ShoppingCartItem item = new ShoppingCartItem();
                        item.Quantity = iq;
                        item.Product = client;
                        ls.Add(item);
                    }
                    var it = from a in ls
                                          where a.Product.ProductID == productId
                                          select a;
                    ShoppingCartItem match = it.FirstOrDefault();
                    if (match!=null)
                    {
                        match.Quantity += quantity;
                    }
                    else
                    {
                        ShoppingCartItem newItem = new ShoppingCartItem();
                        newItem.Product = p;
                        newItem.Quantity = quantity;
                        ls.Add(newItem);
                    }
                    RequestContext.Current.SaveShoppingCartItemsToCookie(ls);
                }
                else
                {
                    ShoppingCartItem newItem = new ShoppingCartItem();
                    newItem.Product = p;
                    newItem.Quantity = quantity;
                    ls.Add(newItem);
                    RequestContext.Current.SaveShoppingCartItemsToCookie(ls);
                }
            }
            else
            {
                DateTime dt = DateTime.Now;
                ShoppingCartItem sp = GetShoppingCartItem(RequestContext.Current.Contact.ContactID, p, e);
                if (sp!=null)
                {
                    sp.Quantity += quantity;
                    sp.ModifiedDate = dt;
                }
                else
                {
                    sp = new ShoppingCartItem();
                    sp.ShoppingCartID = RequestContext.Current.Contact.ContactID.ToString();
                    sp.Quantity += quantity;
                    sp.Product = p;
                    sp.DateCreated = dt;
                    sp.ModifiedDate = dt;
                    e.AddToShoppingCartItem(sp);
                }
                e.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the shopping cart item by ID.
        /// </summary>
        /// <param name="id">The ID of shopping cart item.</param>
        /// <param name="p">The ID of product.</param>
        /// <param name="e">Entities.</param>
        /// <returns></returns>
        public static ShoppingCartItem GetShoppingCartItem(int id, Product p, Entities e)
        {
            string s = id.ToString();
            var shopCartItems = from item in e.ShoppingCartItem
                                where item.ShoppingCartID == s
                                      && item.Product.ProductID == p.ProductID
                                select item;
            return shopCartItems.FirstOrDefault();
        }


        /// <summary>
        /// Gets the shopping cart items.
        /// </summary>
        /// <param name="shoppingCartId">The shopping cart id.</param>
        /// <returns></returns>
        public static List<ShoppingCartItem> GetShoppingCartItems(int shoppingCartId)
        {

            string shoppingCart = "";
            if (shoppingCartId>0)
            {
                shoppingCart = shoppingCartId.ToString();
            }
            if (shoppingCart.Length>0)
            {
                var shopCartItems = from items in Common.DataEntities.ShoppingCartItem
                                    where items.ShoppingCartID == shoppingCart
                                    select items;
                return shopCartItems.ToList();
            }
            List<ShoppingCartItem> list = new List<ShoppingCartItem>();
            HttpCookie cols = RequestContext.Current.CartItemsFromCookie;
            if (cols!=null && cols.Values.Count>0)
            {
                string[] pairs = cols.Values[0].Split(',');
                for (int i = 0; i < pairs.Length; i++)
                {
                    string value = pairs[i];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    string[] ss = value.Split('_');
                    int q = int.Parse(ss[0]);
                    int p = int.Parse(ss[1]);
                    if (q<=0 || p <= 0)
                    {
                        continue;
                    }
                    Product product = ProductManager.GetProductByProductId(p);
                    if (product== null)
                    {
                        continue;
                    }
                    ShoppingCartItem item = new ShoppingCartItem();
                    item.ShoppingCartItemID = -i;
                    item.Quantity = q;
                    item.Product = product;
                    list.Add(item);
                }
            }
            
            return list;
        }

        /// <summary>
        /// Updates the shopping cart item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="quantity">The quantity.</param>
        public static void UpdateShoppingCartItem(int id, int quantity)
        {
            if (RequestContext.Current.Contact == null)
            {
                List<ShoppingCartItem> items = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
                var item = from it in items
                           where it.ShoppingCartItemID == id 
                           select it;
                ShoppingCartItem match = item.FirstOrDefault();

                if (match != null)
                {
                    match.Quantity = quantity;
                }
                RequestContext.Current.SaveShoppingCartItemsToCookie(items);
            }
            else
            {
                Entities entities = Common.DataEntities;
                var shopCartItems = from item in entities.ShoppingCartItem
                                    where item.ShoppingCartItemID == id
                                    select item;
                ShoppingCartItem sp = shopCartItems.FirstOrDefault();
                if (sp == null)
                {
                    return;
                }
                sp.Quantity = quantity;
                entities.SaveChanges();
            }
        }
    }
}