using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using AdventureWorksDataModel;
using System.Collections.Generic;

namespace EpicAdventureWorks
{
    /// <summary>
    /// Summary description for CustomerManager
    /// </summary>
    public class CustomerManager
    {
        /// <summary>
        /// Gets the customer by contact ID.
        /// </summary>
        /// <param name="contactID">The contact ID.</param>
        /// <returns></returns>
        public static Customer GetCustomerByContactID(int contactID)
        {
            var cats = from cat in Common.DataEntities.Customer
                       where cat.Individual.Contact.ContactID == contactID
                       select cat;
            return cats.FirstOrDefault();
        }

        /// <summary>
        /// Gets the customer by contact ID.
        /// </summary>
        /// <param name="contactID">The contact ID.</param>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public static Customer GetCustomerByContactID(int contactID, Entities entities)
        {
            var cats = from cat in entities.Customer
                       where cat.Individual.Contact.ContactID == contactID
                       select cat;
            return cats.FirstOrDefault();
        }

        /// <summary>
        /// Login a customer
        /// </summary>
        /// <param name="email">User name</param>
        /// <param name="password">password</param>
        /// <returns>Result</returns>
        public static bool Login(string email, string password)
        {
            bool login = Membership.ValidateUser(email, password);
            if (login)
            {
                
                // merge shopping items
                List<ShoppingCartItem> ls = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
                
                RequestContext.Current.Contact = ContactManager.GetContactByEmail(email);
                foreach (ShoppingCartItem item in ls)
                {
                    ShoppingCartManager.AddToCart(item.Product.ProductID, item.Quantity);
                    RequestContext.Current.SaveShoppingCartItemsToCookie(new List<ShoppingCartItem>());
                }
            }
            return login;
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        public static void Logout()
        {
            RequestContext.Current.ResetSession();

            if (HttpContext.Current.Session != null)
                HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Gets the customer by ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Customer GetCustomerByID(int id)
        {
            if (id == 0)
            {
                return null;
            }
            var customers = from customer in Common.DataEntities.Customer
                            where customer.CustomerID == id
                            select customer;
            return customers.FirstOrDefault();
        }

        /// <summary>
        /// Adds to customer.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="passWord">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        public static void AddToCustomer(string userName, string passWord, string email, string firstName, string lastName)
        {
            Membership.CreateUser(userName, passWord, email);
            Entities entities = Common.DataEntities;
            // add an individual contact to adventureworks db
            Contact contact = ContactManager.AddContact(passWord, email, firstName, lastName,entities);
            //add customer and invididual to adventureworks db.
            Customer customer = new Customer();
            customer.ModifiedDate = DateTime.Now;
            customer.CustomerType = "I";
            customer.rowguid = Guid.NewGuid();
            entities.AddToCustomer(customer);

            Individual individual = new Individual();
            individual.Contact = contact;
            individual.Customer = customer;
            individual.ModifiedDate = DateTime.Now;
            entities.AddToIndividual(individual);

            entities.SaveChanges();
        }
    }
}
