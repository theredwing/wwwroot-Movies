using System.Linq;
using System.Web;
using System.Web.Security;
using AdventureWorksDataModel;
using System.Web.UI.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EpicAdventureWorks
{

    /// <summary>
    /// Summary description for SalesOrderManager
    /// </summary>
    public class SalesOrderManager
    {

        /// <summary>
        /// Gets the latest sales order info by customer id
        /// </summary>
        /// <param name="customerID">the customer id</param>
        /// <returns></returns>
        public static SalesOrderHeader GetLatestSalesOrderHeaderByCustomerID(int customerID)
        {
            var cats = from cat in Common.DataEntities.SalesOrderHeader
                       where cat.Customer.CustomerID == customerID
                       orderby cat.SalesOrderID descending
                       select new
                       {
                           cat,
                           cat.BillAddress,
                           cat.BillAddress.StateProvince,
                           cat.Contact,
                           cat.CreditCard,
                           cat.Customer,
                           cat.ShipAddress,
                           cat.ShipAddress.StateProvince.StateProvinceCode
                       };
            return cats.FirstOrDefault().cat;
        }

        /// <summary>
        /// Gets the latest sales order info by customer id
        /// </summary>
        /// <param name="customerID">the customer id</param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static SalesOrderHeader GetLatestSalesOrderHeaderByCustomerID(int customerID, Entities entities)
        {
            var cats = from cat in entities.SalesOrderHeader
                       where cat.Customer.CustomerID == customerID
                       orderby cat.SalesOrderID descending
                       select new
                       {
                           cat,
                           cat.BillAddress,
                           cat.BillAddress.StateProvince,
                           cat.Contact,
                           cat.CreditCard,
                           cat.Customer,
                           cat.ShipAddress,
                       };
            return cats.FirstOrDefault().cat;
        }

        /// <summary>
        /// update the credit card info of the sales order.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="creditCard"></param>
        /// <param name="entities"></param>
        public static void UpdateCreditCardInfo(SalesOrderHeader header, CreditCard creditCard, Entities entities)
        {
            var card = (from cards in entities.CreditCard
                        where cards.CardNumber == creditCard.CardNumber
                        select cards).FirstOrDefault();
            if (card == null)
            {
                entities.AddToCreditCard(creditCard);
                header.CreditCard = creditCard;
            }
            else
            {

                header.CreditCard = card;

            }

            entities.SaveChanges();
        }

        /// <summary>
        /// update the shipping info of the sales order
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="contactID"></param>
        /// <param name="shipAddressID"></param>
        /// <param name="shipDate"></param>
        public static void UpdateShippingInfo(int customerID, int contactID, int shipAddressID, DateTime shipDate)
        {
            Entities entities = Common.DataEntities;
            SalesOrderHeader salesOrderHeader = GetLatestSalesOrderHeaderByCustomerID(customerID, entities);
            if (salesOrderHeader == null)
            {
                return;
            }
            salesOrderHeader.Contact = ContactManager.GetContactByContactID(contactID, entities);
            salesOrderHeader.ShipAddress = AddressManager.GetAddressByID(shipAddressID, entities);
            salesOrderHeader.ShipDate = shipDate;
            entities.SaveChanges();
        }

        /// <summary>
        /// add sales order info.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="salesOrderHeader"></param>
        /// <param name="salesOrderDetails"></param>
        /// <param name="productLists"></param>
        public static void AddToSalesOrder(Contact contact, SalesOrderHeader salesOrderHeader, List<SalesOrderDetail> salesOrderDetails, List<string> productLists)
        {
            Entities entities = Common.DataEntities;
            Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID, entities);
            salesOrderHeader.Customer = customer;
            Address address = AddressManager.GetBillAddressByCustomerID(customer.CustomerID, entities);
            if (address == null)
            {
                address = AddressManager.GetFirstAddress(entities);
            }
            salesOrderHeader.BillAddress = address;
            salesOrderHeader.ShipAddress = address;
            salesOrderHeader.Contact = ContactManager.GetContactByContactID(contact.ContactID, entities);
            salesOrderHeader.ShipMethod = GetShipMethod(entities);
            entities.AddToSalesOrderHeader(salesOrderHeader);
            for (int i = 0; i < salesOrderDetails.Count; i++)
            {
                SalesOrderDetail detail = salesOrderDetails[i];
                detail.SalesOrderHeader = salesOrderHeader;
                //detail.Product = ProductManager.GetProductByProductId(Int32.Parse(productLists[i]), entities);
                SpecialOfferProduct prod = GetSpecialOfferProduct(Int32.Parse(productLists[i]), entities);
                if (prod == null)
                {
                    AddToSpecialOfferProduct(prod, Int32.Parse(productLists[i]), entities);
                }
                detail.SpecialOfferProduct = prod;
                entities.AddToSalesOrderDetail(detail);
            }
            entities.SaveChanges();
        }

        private static ShipMethod GetShipMethod(Entities entities)
        {
            var cats = from cat in entities.ShipMethod
                       where cat.ShipMethodID == 5
                       select cat;
            return cats.FirstOrDefault();
        }

        private static void AddToSpecialOfferProduct(SpecialOfferProduct soProduct, int productID, Entities entities)
        {
            soProduct = new SpecialOfferProduct();
            soProduct.ProductID = productID;
            soProduct.SpecialOfferID = 1;
            entities.AddToSpecialOfferProduct(soProduct);
        }

        private static SpecialOfferProduct GetSpecialOfferProduct(int productID, Entities entities)
        {
            var cats = from cat in entities.SpecialOfferProduct
                       where cat.ProductID == productID
                       select cat;
            return cats.FirstOrDefault();
        }

    }

}