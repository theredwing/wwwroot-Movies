using System;
using System.Collections.Generic;
using System.Web;
using AdventureWorksDataModel;
using System.Collections.Specialized;

namespace EpicAdventureWorks
{
    /// <summary>
    /// The requesting context object. Contains information about current customer and shopping cart.
    /// </summary>
    public class RequestContext
    {
        private const string CONST_CUSTOMERSESSIONCOOKIE = "Cookie-CustomerSessionGUID";
        private HttpContext context = HttpContext.Current;
        private Contact _contact;

        /// <summary>
        /// Gets the user shopping cart.
        /// </summary>
        /// <value>The user shopping cart.</value>
        public ShoppingCart UserShoppingCart
        {
            get
            {
                ShoppingCart c = this["Cart"] as ShoppingCart;
                if (c!=null)
                {
                    return c;
                }
                ShoppingCart cart = new ShoppingCart(Contact);
                this["Cart"] = cart;
                return cart;
            }

        }

        /// <summary>
        /// Gets or sets the contact, if returns null, it means request is from an anonymous contact.
        /// </summary>
        /// <value>The contact.</value>
        public Contact Contact
        {
            get
            {
                bool authenticated = false;
                if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                {
                    authenticated = HttpContext.Current.User.Identity.IsAuthenticated;
                }
                if (authenticated)
                {
                    string name = HttpContext.Current.User.Identity.Name;
                    _contact = ContactManager.GetContactByEmail(name);
                }
                return _contact;
            }
            set
            {
                _contact = value;
            }
        }

        /// <summary>
        /// Gets the current contact context.
        /// </summary>
        /// <value>The current.</value>
        public static RequestContext Current
        {
            get
            {
                RequestContext r = HttpContext.Current.Items["Request"] as RequestContext;
                if (r == null)
                {
                    r = new RequestContext();
                    HttpContext.Current.Items["Request"] = r;
                }
                return r;
            }
        }

        /// <summary>
        /// Reset customer session
        /// </summary>
        public void ResetSession()
        {
            if (HttpContext.Current != null)
            {
                Common.SetCookie(HttpContext.Current.ApplicationInstance, CONST_CUSTOMERSESSIONCOOKIE, string.Empty);
            }
            Contact = null;
            this["SessionReset"] = true;
        }

        /// <summary>
        /// Gets or sets an object item in the context by the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public object this[string key]
        {
            get
            {
                if (context == null)
                {
                    return null;
                }

                if (context.Items[key] != null)
                {
                    return context.Items[key];
                }
                return null;
            }
            set
            {
                if (context != null)
                {
                    context.Items.Remove(key);
                    context.Items.Add(key, value);

                }
            }
        }

        /// <summary>
        /// Gets the cart items from cookie.
        /// </summary>
        /// <value>The cart items from cookie.</value>
        public HttpCookie CartItemsFromCookie
        {
            get
            {
                HttpCookie co = HttpContext.Current.Request.Cookies[CONST_CUSTOMERSESSIONCOOKIE];
                if (Current["ResponseCookie"] is bool && (bool)Current["ResponseCookie"])
                {
                    co = HttpContext.Current.Response.Cookies[CONST_CUSTOMERSESSIONCOOKIE];
                }
                return co;
            }
        }

        /// <summary>
        /// Saves the shopping cart items to cookie.
        /// </summary>
        /// <param name="items">The items.</param>
        public void SaveShoppingCartItemsToCookie(List<ShoppingCartItem> items)
        {
            if (items == null)
            {
                return;
            }
            HttpCookie k = new HttpCookie(CONST_CUSTOMERSESSIONCOOKIE);
            foreach (ShoppingCartItem item in items)
            {
                k.Values.Add("ShoppingCartItem",item.Quantity + "_" + item.Product.ProductID);
            }
            Common.SetCookie(k);
            Current["ResponseCookie"] = true;
        }
    }
}