using AdventureWorksDataModel;
using EpicAdventureWorks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdventureWorks.Models;
using System.Data;
using System.IO;
using C1.Web.Wijmo.Controls.C1ReportViewer;

namespace AdventureWorks.Controllers
{
    public class ShoppingCartController : Controller
    {

        public ActionResult OpenCart()
        {
            return View();
        }

        public ActionResult ShopCart()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            if (RequestContext.Current.Contact == null)
            {
                Response.Redirect("/Home/Login");
            }
            Contact contact = RequestContext.Current.Contact;
            if (contact != null)
            {
                Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID);
                BillAddress = AddressManager.GetBillAddressByCustomerID(customer.CustomerID);
                if (BillAddress == null) BillAddress = CreateDefaultBillAddress();
            }
            return View();
        }

        public ActionResult Shipping()
        {
            if (RequestContext.Current.Contact == null)
            {
                Response.Redirect("/Home/Login");
            }
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            return View();
        }

        public ActionResult ReviewOrder()
        {
            if (RequestContext.Current.Contact == null)
            {
                Response.Redirect("/Home/Login");
            }
            ReviewOrderModel Model = new ReviewOrderModel();
            Contact cont = RequestContext.Current.Contact;
            Customer customer = CustomerManager.GetCustomerByContactID(cont.ContactID);
            SalesOrderHeader salesOrderHeader = SalesOrderManager.GetLatestSalesOrderHeaderByCustomerID(customer.CustomerID);
            Model.Subtotal = "$" + salesOrderHeader.SubTotal.ToString("N2");
            Model.Feight = "$" + salesOrderHeader.Freight.ToString("N2");
            Model.TotalDue = "$" + salesOrderHeader.TotalDue.ToString("N2");

            //bill info
            Address billAddress = salesOrderHeader.BillAddress;
            Model.BillFirstName = cont.FirstName;
            Model.BillLastName = cont.LastName;
            Model.BillCreditCardType = salesOrderHeader.CreditCard.CardType;
            Model.BillCreditCardNumber = salesOrderHeader.CreditCard.CardNumber;
            string expireMonth = salesOrderHeader.CreditCard.ExpMonth.ToString(CultureInfo.InvariantCulture);
            string expireYear = salesOrderHeader.CreditCard.ExpYear.ToString(CultureInfo.InvariantCulture);
            if (expireYear.Length < 2)
            {
                expireYear = DateTime.Now.Year.ToString("YYYY");
            }
            Model.BillCreditCardExpire = expireMonth + "/" + expireYear.Substring(expireYear.Length - 2);
            Model.BillAddressLine1 = billAddress.AddressLine1;
            Model.BillAddressLine2 = billAddress.AddressLine2;
            Model.BillAddress = billAddress.City + ", " + billAddress.StateProvince.StateProvinceCode + " " + billAddress.PostalCode;

            //ship info
            Address shipAddress = AddressManager.GetAddressByID(salesOrderHeader.ShipAddress.AddressID);
            Contact contact = salesOrderHeader.Contact;
            Model.ShipFirstName = contact.FirstName;
            Model.ShipLastName = contact.LastName;
            Model.ShipAddressLine1 = shipAddress.AddressLine1;
            Model.ShipAddressLine2 = shipAddress.AddressLine2;
            Model.ShipAddress = shipAddress.City + ", " + shipAddress.StateProvince.StateProvinceCode + " " + shipAddress.PostalCode;
            Model.ShipPhone = contact.Phone;
            Model.ShipEmail = contact.EmailAddress;
            return View(Model);
        }

        public ActionResult OrderComplete()
        {
            if (RequestContext.Current.Contact == null)
            {
                Response.Redirect("/Home/Login");
            }
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");
            if (C1ReportViewer.IsDocumentRegistered(_reportName))
            {
                C1ReportViewer.UnRegisterDocument(_reportName);
            }
            _reportName = ReportNamePrefix + DateTime.UtcNow.Ticks;
            C1ReportViewer.RegisterDocument(_reportName, GetReport);
            ViewBag.ReportFileName = _reportName;
            return View();
        }

        private Address CreateDefaultBillAddress()
        {
            var random = new Random();
            return new Address
            {
                AddressLine1 = random.Next(1, 999) + " S Highland Ave",
                AddressLine2 = "3rd Floor",
                City = "Pittsburgh",
                StateProvince = new StateProvince
                {
                    StateProvinceCode = "PA",
                    Name = "Pennsylvania"
                },
                PostalCode = random.Next(1, 9) + random.Next(0, 9999).ToString("0000")
            };
        }

        

        private const string ReportNamePrefix = "ReceiptReport";
        private static string _reportName = ReportNamePrefix;
        
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <returns></returns>
        private object GetReport()
        {
            Contact contact = RequestContext.Current.Contact;
            Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID);
            SalesOrderHeader salesOrderHeader = SalesOrderManager.GetLatestSalesOrderHeaderByCustomerID(customer.CustomerID);

            C1.C1Report.C1Report rep = C1ReportViewer.CreateC1Report();
            rep.UseGdiPlusTextRendering = true;
            rep.EmfType = System.Drawing.Imaging.EmfType.EmfPlusOnly;
            // load report into control
            string reportFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "adventureworks.xml");
            rep.Load(reportFile, "ReceiptReport");
            DataView dv = GetReportRecordSet(salesOrderHeader);
            rep.DataSource.Recordset = dv;
            foreach (C1.C1Report.Field f in rep.Fields)
            {
                C1.C1Report.C1Report sr = f.Subreport;
                if (sr == null) continue;
                sr.DataSource.Recordset = GetReportGridRecordSet(salesOrderHeader);
            }
            return rep;
        }

        private DataView GetReportRecordSet(SalesOrderHeader salesOrderHeader)
        {
            salesOrderHeader.BillAddressReference.Load();

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("BillingAddress", typeof(string));
            dataTable.Columns.Add("BillingAddress2", typeof(string));
            dataTable.Columns.Add("BillingCity", typeof(string));
            dataTable.Columns.Add("BillingPostalCode", typeof(string));
            dataTable.Columns.Add("BillingStateProvinceCode", typeof(string));

            dataTable.Columns.Add("ShippingAddress", typeof(string));
            dataTable.Columns.Add("ShippingAddress2", typeof(string));
            dataTable.Columns.Add("ShippingCity", typeof(string));
            dataTable.Columns.Add("ShippingPostalCode", typeof(string));
            dataTable.Columns.Add("ShippingStateProvinceCode", typeof(string));
            dataTable.Columns.Add("SubTotal", typeof(decimal));
            dataTable.Columns.Add("Freight", typeof(decimal));

            dataTable.Columns.Add("FullName", typeof(string));
            dataTable.Columns.Add("EmailAddress", typeof(string));
            dataTable.Columns.Add("Phone", typeof(string));
            dataTable.Columns.Add("CardType", typeof(string));
            dataTable.Columns.Add("CardNumber", typeof(string));


            DataRow dr = dataTable.NewRow();
            dr["BillingAddress"] = salesOrderHeader.BillAddress.AddressLine1;
            dr["BillingAddress2"] = salesOrderHeader.BillAddress.AddressLine2;
            dr["BillingCity"] = salesOrderHeader.BillAddress.City;
            dr["BillingPostalCode"] = salesOrderHeader.BillAddress.PostalCode;
            salesOrderHeader.BillAddress.StateProvinceReference.Load();
            dr["BillingStateProvinceCode"] = salesOrderHeader.BillAddress.StateProvince.StateProvinceCode;

            dr["ShippingAddress"] = salesOrderHeader.ShipAddress.AddressLine1;
            dr["ShippingAddress2"] = salesOrderHeader.ShipAddress.AddressLine2;
            dr["ShippingCity"] = salesOrderHeader.ShipAddress.City;
            dr["ShippingPostalCode"] = salesOrderHeader.ShipAddress.PostalCode;
            salesOrderHeader.ShipAddress.StateProvinceReference.Load();
            dr["ShippingStateProvinceCode"] = salesOrderHeader.ShipAddress.StateProvince.StateProvinceCode;
            dr["SubTotal"] = salesOrderHeader.SubTotal;
            dr["Freight"] = salesOrderHeader.Freight;
            salesOrderHeader.ContactReference.Load();
            dr["FullName"] = salesOrderHeader.Contact.FirstName + " " + salesOrderHeader.Contact.LastName;
            dr["EmailAddress"] = salesOrderHeader.Contact.EmailAddress;
            dr["Phone"] = salesOrderHeader.Contact.Phone;
            salesOrderHeader.CreditCardReference.Load();
            dr["CardType"] = salesOrderHeader.CreditCard.CardType;
            dr["CardNumber"] = salesOrderHeader.CreditCard.CardNumber;
            dataTable.Rows.Add(dr);
            return dataTable.DefaultView;
        }

        private DataView GetReportGridRecordSet(SalesOrderHeader salesOrderHeader)
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SalesOrderID", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("UnitPrice", typeof(decimal));
            dataTable.Columns.Add("OrderQty", typeof(short));
            dataTable.Columns.Add("LineTotal", typeof(decimal));

            salesOrderHeader.SalesOrderDetail.Load();
            IEnumerator<SalesOrderDetail> dt = salesOrderHeader.SalesOrderDetail.GetEnumerator();
            while (dt.MoveNext())
            {
                DataRow dr1 = dataTable.NewRow();
                Debug.Assert(dt.Current != null, "dt.Current != null");
                dr1["SalesOrderID"] = dt.Current.SalesOrderID;
                dt.Current.SpecialOfferProductReference.Load();
                dt.Current.SpecialOfferProduct.ProductReference.Load();
                dr1["Name"] = dt.Current.SpecialOfferProduct.Product.Name;
                dr1["UnitPrice"] = dt.Current.UnitPrice;
                dr1["OrderQty"] = dt.Current.OrderQty;
                dr1["LineTotal"] = dt.Current.LineTotal;

                dataTable.Rows.Add(dr1);
            }

            return dataTable.DefaultView;
        }

        /// <summary>
        /// To Get Shopping Cart Items Count
        /// </summary>
        public JsonResult GetCartItemsCount()
        {
            try
            {
                string CartItemsCount = "0";
                CartItemsCount = RequestContext.Current.UserShoppingCart.ShoppingCarItems.Count.ToString();
                return Json(new { ResultType = "Success", ErrMsg = "N/A", CartItemsCount = CartItemsCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        /// <summary>
        /// To Get Billing Address
        /// </summary>
        public JsonResult GetBillAddress()
        {
            try
            {
                Contact contact = RequestContext.Current.Contact;
                Address address = null;
                if (contact != null)
                {
                    Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID);
                    address = AddressManager.GetBillAddressByCustomerID(customer.CustomerID);
                    if (address == null)
                    {
                        address = BillAddress;
                    }

                    if (address != null)
                    {
                        return Json(new { ResultType = "Success", ErrMsg = "N/A", BillContact = contact, Address="Yes", AddressLine1 = address.AddressLine1, AddressLine2 = address.AddressLine2, City = address.City, StateCode = address.StateProvince.StateProvinceCode.Trim(), StateName = address.StateProvince.Name.Trim(), Zip = address.PostalCode }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { ResultType = "Success", ErrMsg = "N/A", BillContact = contact, Address = "No" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { ResultType = "Success", ErrMsg = "No Address found." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// To Get ReviewOrder GridData
        /// </summary>
        public JsonResult GetReviewOrderGridData()
        {
            try
            {
                Contact contact = RequestContext.Current.Contact;
                Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID);
                int salesOrderId = SalesOrderManager.GetLatestSalesOrderHeaderByCustomerID(customer.CustomerID).SalesOrderID;

                Entities entities = Common.DataEntities;
                var items = from item in entities.SalesOrderDetail
                            join p in entities.Product on item.SpecialOfferProduct.ProductID equals p.ProductID
                            where item.SalesOrderID == salesOrderId
                            select new
                            {
                                item.SalesOrderDetailID,
                                p.Name,
                                item.UnitPrice,
                                item.OrderQty,
                                item.LineTotal
                            };
                return Json(new { ResultType = "Success", ErrMsg = "", GridData = items.ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// To Update item qty in Shopping Cart
        /// </summary>
        public JsonResult UpdateItemQuantity(int cartItemId, int quantity)
        {
            try
            {
                ShoppingCartManager.UpdateShoppingCartItem(cartItemId, quantity);
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = "false", error = "Update failed. Detail: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// To Remove item from Shopping Cart
        /// </summary>
        public JsonResult RemoveItem(int cartItemId)
        {
            try
            {
                ShoppingCartManager.DeleteShoppingCartItem(cartItemId);
                return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = "false", error = "Remove failed. Detail: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// To Get OrderSummary GridData
        /// </summary>
        public JsonResult GetOrderSummaryGridData()
        {
            try
            {
                List<ShoppingCartItem> ls = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
                var binddata = from i in ls
                               select new { i.ShoppingCartItemID, i.Product.ProductID, i.Product.Name, i.Product.ListPrice, i.Quantity, Cost = i.Product.ListPrice * i.Quantity };
                var subTotal = binddata.Sum(item => item.Cost);
                return Json(new { ResultType = "Success", ErrMsg = "", GridData = binddata.ToList() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// To Get OrderSummary TotalData
        /// </summary>
        public JsonResult GetOrderSummaryTotalData(string ShippingCharges)
        {
            try
            {
                List<ShoppingCartItem> ls = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
                var binddata = from i in ls
                               select new { i.ShoppingCartItemID, i.Product.ProductID, i.Product.Name, i.Product.ListPrice, i.Quantity, Cost = i.Product.ListPrice * i.Quantity };
                var subTotal = binddata.Sum(item => item.Cost);
                string SubTotalAmount = subTotal.ToString("C"), ShippingAmount = decimal.Parse(ShippingCharges).ToString("C");
                string TotalAmount = (subTotal + decimal.Parse(ShippingCharges)).ToString("C");
                return Json(new { ResultType = "Success", ErrMsg = "", SubTotalAmount = SubTotalAmount, ShippingAmount = ShippingAmount, TotalAmount = TotalAmount }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult CheckOutClick(string ShippingCharges)
        {
            try
            {
                if (RequestContext.Current.Contact == null)
                {
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", RedirectURL = "/Home/Login" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Contact contact = RequestContext.Current.Contact;
                    SalesOrderHeader header = SetOrderHeader(ShippingCharges);
                    List<string> productIds = new List<string>();
                    List<SalesOrderDetail> detailList = SetOrderDetail(productIds);
                    if (productIds.Count > 0)
                    {
                        SalesOrderManager.AddToSalesOrder(contact, header, detailList, productIds);
                        //Delete from ShopCartItem.
                        ShoppingCartManager.DeleteShoppingCartItemByCartID(contact.ContactID.ToString(CultureInfo.InvariantCulture));
                        return Json(new { ResultType = "Success", ErrMsg = "N/A", RedirectURL = "/ShoppingCart/CheckOut" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { ResultType = "Success", ErrMsg = "Please select a product first.", RedirectURL = "N/A" }, JsonRequestBehavior.AllowGet);
                    }
                }
                
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private Address BillAddress
        {
            get
            {
                if (Session["BillAddress"] == null)
                {
                    return null;
                }
                return (Address)Session["BillAddress"];
            }
            set
            {
                Session["BillAddress"] = value;
            }
        }

        public JsonResult CheckOut_btnNextClick(string Email, string FirstName, string LastName, string Phone, string AddressLine1, string AddressLine2, string City, string State, string Zip, string ExpirationYear, string ExpirationMonth, string CardNumber, string PaymentType, string Code)
        {
            try
            {
                if (RequestContext.Current.Contact == null)
                {
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", RedirectURL = "/Home/Login" }, JsonRequestBehavior.AllowGet);
                }
                SaveCustomerAndAddressInfo(Email, FirstName, LastName, Phone, AddressLine1, AddressLine2, City, State, Zip);
                SaveSalesOrderCreditCardInfo(ExpirationYear, ExpirationMonth, CardNumber, PaymentType, Code);
                return Json(new { ResultType = "Success", ErrMsg = "N/A", RedirectURL = "/ShoppingCart/Shipping" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //save address and customer info.
        private void SaveCustomerAndAddressInfo(string Email, string FirstName, string LastName, string Phone, string AddressLine1, string AddressLine2, string City, string State, string Zip)
        {
            Contact contact = RequestContext.Current.Contact;
            contact.EmailAddress = Email;
            contact.FirstName = FirstName;
            contact.LastName = LastName;
            contact.Phone = Phone;
            ContactManager.SaveChanges(contact);
            Entities entities = Common.DataEntities;
            if (BillAddress == null)
            {
                AddAddress(entities, contact,AddressLine1,  AddressLine2,  City,  State,  Zip);
                return;
            }

            Address address = AddressManager.GetAddressByID(BillAddress.AddressID, entities);
            if (address == null)
            {
                AddAddress(entities, contact, AddressLine1, AddressLine2, City, State, Zip);
                return;
            }

            SetAddressInfo(address, entities, AddressLine1, AddressLine2, City, State, Zip);
            AddressManager.SaveChanges(address);
            BillAddress = address;
        }

        private void AddAddress(Entities entities, Contact contact, string AddressLine1, string AddressLine2, string City, string State, string Zip)
        {
            Address address = new Address();
            SetAddressInfo(address, entities, AddressLine1, AddressLine2, City, State, Zip);
            Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID, entities);
            BillAddress = AddressManager.AddToAddress("Bill", address, customer, entities);
        }

        private void SetAddressInfo(Address address, Entities entities, string AddressLine1, string AddressLine2, string City, string State, string Zip)
        {
            address.AddressLine1 = AddressLine1;
            address.AddressLine2 = AddressLine2;
            address.City = City;
            StateProvince stateProvince = AddressManager.GetStateProvinceFromCode(State, entities);
            address.StateProvince = stateProvince;
            address.PostalCode = Zip;
            address.ModifiedDate = DateTime.Now;
            address.rowguid = Guid.NewGuid();
        }

        private void SaveSalesOrderCreditCardInfo(string ExpirationYear, string ExpirationMonth, string CardNumber, string PaymentType, string Code)
        {
            Contact contact = RequestContext.Current.Contact;
            Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID);
            if (customer == null)
            {
                return;
            }
            Entities entities = Common.DataEntities;
            SalesOrderHeader salesOrderHeader = SalesOrderManager.GetLatestSalesOrderHeaderByCustomerID(customer.CustomerID,
                entities);
            Address addr = AddressManager.GetAddressByID(BillAddress.AddressID, entities);
            salesOrderHeader.BillAddress = addr;
            salesOrderHeader.ShipAddress = addr;
            CreditCard creditCard = new CreditCard();
            creditCard.CardNumber = CardNumber;
            creditCard.CardType = PaymentType;
            creditCard.ExpMonth = (byte)(int.Parse(ExpirationMonth));
            creditCard.ExpYear = (short)(int.Parse(ExpirationMonth));
            creditCard.ModifiedDate = DateTime.Now;
            salesOrderHeader.CreditCardApprovalCode = Code.ToString(CultureInfo.InvariantCulture);
            SalesOrderManager.UpdateCreditCardInfo(salesOrderHeader, creditCard, entities);
        }

        private SalesOrderHeader SetOrderHeader(string ShippingCharges)
        {
            SalesOrderHeader header = new SalesOrderHeader();
            header.RevisionNumber = 1;
            header.ModifiedDate = header.OrderDate = header.DueDate = DateTime.Now;
            header.Status = 5;
            header.OnlineOrderFlag = false;
            header.SalesOrderNumber = "SO71774";

            List<ShoppingCartItem> ls = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
            var binddata = from i in ls
                           select new { i.ShoppingCartItemID, i.Product.ProductID, i.Product.Name, i.Product.ListPrice, i.Quantity, Cost = i.Product.ListPrice * i.Quantity };
            var subTotal = binddata.Sum(item => item.Cost);
            string SubTotalAmount = subTotal.ToString("C"), ShippingAmount = decimal.Parse(ShippingCharges).ToString("C");
            string TotalAmount = (subTotal + decimal.Parse(ShippingCharges)).ToString("C");
            header.SubTotal = subTotal;
            header.TaxAmt = 0;
            header.Freight = decimal.Parse(ShippingCharges); 
            header.TotalDue = header.SubTotal + header.Freight;
            header.rowguid = Guid.NewGuid();
            return header;
        }

        private List<SalesOrderDetail> SetOrderDetail(List<string> productIds)
        {
            List<SalesOrderDetail> list = new List<SalesOrderDetail>();
            List<ShoppingCartItem> ls = RequestContext.Current.UserShoppingCart.ShoppingCarItems;
            for (int i = 0; i < ls.Count(); i++)
            {
                SalesOrderDetail detail = new SalesOrderDetail();
                ShoppingCartItem shopCartItem = ls[i];
                if (shopCartItem == null) continue;

                int quantity = shopCartItem.Quantity;
                decimal unitPrice = shopCartItem.Product.ListPrice;
                detail.OrderQty = (short)quantity;
                detail.UnitPrice = unitPrice;
                detail.UnitPriceDiscount = 0;
                detail.LineTotal = quantity * unitPrice;
                detail.ModifiedDate = DateTime.Now;
                detail.rowguid = Guid.NewGuid();
                list.Add(detail);

                productIds.Add(ls[i].Product.ProductID.ToString());
            }
            return list;
        }

        public JsonResult Shipping_btnNextClick(string Email, string FirstName, string LastName, string Phone, string AddressLine1, string AddressLine2, string City, string State, string Zip, string ShipDate, string SameAddress)
        {
            try
            {
                Contact contact = RequestContext.Current.Contact;
                if (contact == null)
                {
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", RedirectURL = "/Home/Login" }, JsonRequestBehavior.AllowGet);
                }

                DateTime dt = DateTime.Parse(ShipDate);
                if (dt < DateTime.Now)
                {
                    dt = DateTime.Now;
                }
                int addressId;

                int contactId = contact.ContactID;
                Customer customer = CustomerManager.GetCustomerByContactID(contact.ContactID);
                if (SameAddress != "1")
                {
                    //Save ship address.
                    contactId = SaveContact(FirstName,  LastName,  Email,  Phone);
                    addressId = SaveShipAddress(AddressLine1, AddressLine2, City, State, Zip);
                }
                else
                {
                    addressId = AddressManager.GetBillAddressByCustomerID(customer.CustomerID).AddressID;
                }
                //update salesOrder.
                SalesOrderManager.UpdateShippingInfo(customer.CustomerID, contactId, addressId, dt);
                return Json(new { ResultType = "Success", ErrMsg = "N/A", RedirectURL = "/ShoppingCart/ReviewOrder" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        private int SaveContact(string FirstName, string LastName, string Email, string Phone)
        {
            Contact contact = new Contact();
            contact.FirstName = FirstName;
            contact.LastName = LastName;
            contact.EmailAddress = Email;
            contact.Phone = Phone;
            contact.rowguid = Guid.NewGuid();
            contact.ModifiedDate = DateTime.Now;
            contact.PasswordHash = "1";
            contact.PasswordSalt = "1";
            Entities entities = Common.DataEntities;
            entities.AddToContact(contact);
            entities.SaveChanges();
            return contact.ContactID;
        }

        private int SaveShipAddress(string AddressLine1, string AddressLine2,string City,string State,string Zip)
        {
            Entities entities = Common.DataEntities;
            Address address = new Address();
            address.AddressLine1 = AddressLine1;
            address.AddressLine2 = AddressLine2;
            address.City = City;

            address.StateProvince = AddressManager.GetStateProvinceFromCode(State, entities);
            address.PostalCode = Zip;
            address.rowguid = Guid.NewGuid();
            address.ModifiedDate = DateTime.Now;
            Address dbAddress = AddressManager.GetAddress(address);
            if (dbAddress == null)
            {
                entities.AddToAddress(address);
                entities.SaveChanges();
                return address.AddressID;
            }
            return dbAddress.AddressID;
        }

        private Address GetAddressFromPage(Entities entities)
        {
            Address address = new Address();
            return address;
        }


    }
}
