using System.Linq;
using System.Web;
using System.Web.Security;
using AdventureWorksDataModel;
using System.Web.UI.WebControls;


namespace EpicAdventureWorks
{

    /// <summary>
    /// Summary description for AddressManager
    /// </summary>
    public class AddressManager
    {

        public static Address GetAddress(Address address)
        {
            Entities entities = Common.DataEntities;
            var cats = from cat in entities.Address
                       where cat.AddressLine1 == address.AddressLine1 && cat.AddressLine2 == address.AddressLine2
                            && cat.City == address.City && cat.PostalCode == address.PostalCode
                            && cat.StateProvince.StateProvinceCode == address.StateProvince.StateProvinceCode
                       select cat;
            return cats.FirstOrDefault();
        }

        public static Address GetFirstAddress(Entities entities)
        {
            var cats = from cat in entities.Address
                       select cat;
            return cats.FirstOrDefault();
        }

        /// <summary>
        /// Get StateProvince from StateProvinceCode
        /// </summary>
        /// <param name="provinceCode">the province code</param>
        /// <param name="entities">the entities</param>
        /// <returns>state province</returns>
        public static StateProvince GetStateProvinceFromCode(string provinceCode,Entities entities)
        {
            var cats = from cat in entities.StateProvince
                       where cat.StateProvinceCode == provinceCode
                       select cat;
            return cats.FirstOrDefault();
        }

        private static AddressType GetAddressType(string addressType,Entities entities)
        {
            var cats = from cat in entities.AddressType
                       where cat.Name == addressType
                       select cat;
            return cats.FirstOrDefault();
        }

        /// <summary>
        /// get bill address by customer id
        /// </summary>
        /// <param name="customerId">the customer id.</param>
        /// <returns>address</returns>
        public static Address GetBillAddressByCustomerID(int customerId)
        {
            Entities entities = Common.DataEntities;
            AddressType addressType = GetAddressType("Billing", entities);
            var cats = from cat in entities.CustomerAddress
                       where cat.CustomerID == customerId && cat.AddressType.AddressTypeID == addressType.AddressTypeID
                       orderby cat.Address.AddressID descending
                       select new
                       {
                           cat.Address,
                           cat.Address.StateProvince,
                           cat.AddressType
                       };
            if (cats.FirstOrDefault() == null)
            {
                return null;
            }
            return cats.FirstOrDefault().Address;
        }

        /// <summary>
        /// 
        /// </summary>
        /// get bill address by customer id
        /// </summary>
        /// <param name="customerId">the customer id.</param>
        /// <param name="entities">the entity</param>
        /// <returns></returns>
        public static Address GetBillAddressByCustomerID(int customerId,Entities entities)
        {
            AddressType addressType = GetAddressType("Billing", entities);
            var cats = from cat in entities.CustomerAddress
                       where cat.CustomerID == customerId && cat.AddressType.AddressTypeID == addressType.AddressTypeID
                       orderby cat.Address.AddressID descending
                       select new
                       {
                           cat.Address,
                           cat.Address.StateProvince,
                           cat.AddressType
                       };
            if (cats.FirstOrDefault() == null)
            {
                return null;
            }
            return cats.FirstOrDefault().Address;
        }

        /// <summary>
        /// get address by address id
        /// </summary>
        /// <param name="addressId"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static Address GetAddressByID(int addressId,Entities entities)
        {
            var cats = from cat in entities.Address
                       where cat.AddressID == addressId
                       select new
                       {
                           cat,
                           cat.StateProvince
                       };
            if (cats.FirstOrDefault() == null)
            {
                return null;
            }
            return cats.FirstOrDefault().cat;
        }

        /// <summary>
        /// get address by address id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public static Address GetAddressByID(int addressId)
        {
            var cats = from cat in Common.DataEntities.Address
                       where cat.AddressID == addressId
                       select new 
                       { 
                           cat,
                           cat.StateProvince
                       };
            if (cats.FirstOrDefault() == null)
            {
                return null;
            }
            return cats.FirstOrDefault().cat;
        }

        /// <summary>
        /// save the change of address.
        /// </summary>
        /// <param name="address"></param>
        public static void SaveChanges(Address address)
        {
            Entities entities = Common.DataEntities;

            var cats = from cat in entities.Address
                       where cat.AddressID == address.AddressID
                       select cat;
            Address addr = cats.FirstOrDefault();
            Clone(address, addr, entities);
            entities.SaveChanges();
        }

        /// <summary>
        /// clone an address data.
        /// </summary>
        /// <param name="desAddress">the destination </param>
        /// <param name="tarAddress">the target</param>
        /// <param name="entities"></param>
        public static void Clone(Address desAddress,Address tarAddress,Entities entities)
        {
            tarAddress.AddressLine1 = desAddress.AddressLine1;
            tarAddress.AddressLine2 = desAddress.AddressLine2;
            tarAddress.City = desAddress.City;
            //tarAddress.CountryRegion = desAddress.CountryRegion;
            tarAddress.PostalCode = desAddress.PostalCode;
            StateProvince stateProvince = AddressManager.GetStateProvinceFromCode(desAddress.StateProvince.StateProvinceCode, entities);
            tarAddress.StateProvince = stateProvince;
        }

        /// <summary>
        /// add an address data.
        /// </summary>
        /// <param name="addressType"></param>
        /// <param name="address"></param>
        /// <param name="customer"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static Address AddToAddress(string addressType,Address address,Customer customer,Entities entities)
        {
            entities.AddToAddress(address);
            CustomerAddress cAddr = new CustomerAddress();
            cAddr.ModifiedDate = System.DateTime.Now;
            //cAddr.AddressType = addressType;
            cAddr.rowguid = System.Guid.NewGuid();
            cAddr.Address = address;

            if (addressType == "Bill")
            {
                cAddr.Customer = customer;
                AddressType addrType = GetAddressType("Billing", entities);
                cAddr.AddressType = addrType;
            }
            else if (addressType == "Ship")
            {
                //cAddr.Customer = customer;
            }

            entities.AddToCustomerAddress(cAddr);
            entities.SaveChanges();

            return address;

        }

    }

}