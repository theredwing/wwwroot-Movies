using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Security;
using AdventureWorksDataModel;

namespace EpicAdventureWorks
{
    /// <summary>
    /// Summary description for ContactManager
    /// </summary>
    public class ContactManager
    {
        /// <summary>
        /// Gets the contact by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static Contact GetContactByEmail(string email)
        {
            var contact = from c in Common.DataEntities.Contact
                          where c.EmailAddress == email
                          select c;
            return contact.FirstOrDefault();
        }

        /// <summary>
        /// Gets the contact by contact id
        /// </summary>
        /// <param name="contactID">The contactID</param>
        /// <param name="entities">the entities</param>
        /// <returns></returns>
        public static Contact GetContactByContactID(int contactID, Entities entities)
        {
            var contact = from c in entities.Contact
                          where c.ContactID == contactID
                          select c;
            return contact.FirstOrDefault();
        }

        /// <summary>
        /// Adds the contact.
        /// </summary>
        /// <param name="passWord">The pass word.</param>
        /// <param name="email">The email.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        public static void AddContact(string passWord, string email, string firstName, string lastName)
        {
            Contact c = new Contact();
            string salt = CreateSalt(4);
            c.NameStyle = false;
            c.FirstName = firstName;
            c.LastName = lastName;

            c.PasswordHash = CreatePasswordHash(passWord, salt);
            c.PasswordSalt = salt;
            c.EmailAddress = email;
            c.ModifiedDate = DateTime.Now;
            c.rowguid = Guid.NewGuid();
            Entities e = Common.DataEntities;
            e.AddToContact(c);
            e.SaveChanges();
        }

        /// <summary>
        /// Adds the contact.
        /// </summary>
        /// <param name="passWord">The pass word.</param>
        /// <param name="email">The email.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="entities">The entities</param>
        public static Contact AddContact(string passWord, string email, string firstName, string lastName,Entities entities)
        {
            Contact c = new Contact();
            string salt = CreateSalt(4);
            c.NameStyle = false;
            c.FirstName = firstName;
            c.LastName = lastName;

            c.PasswordHash = CreatePasswordHash(passWord, salt);
            c.PasswordSalt = salt;
            c.EmailAddress = email;
            c.ModifiedDate = DateTime.Now;
            c.rowguid = Guid.NewGuid();
            entities.AddToContact(c);
            return c;
        }

        /// <summary>
        /// Creates a password hash
        /// </summary>
        /// <param name="Password">Password</param>
        /// <param name="Salt">Salt</param>
        /// <returns>Password hash</returns>
        private static string CreatePasswordHash(string password, string salt)
        {
            //MD5, SHA1
            string passwordFormat = "SHA1";
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, passwordFormat);
        }

        /// <summary>
        /// Creates a salt
        /// </summary>
        /// <param name="size">A salt size</param>
        /// <returns>A salt</returns>
        private static string CreateSalt(int size)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            byte[] data = new byte[size];
            provider.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Save change of the contact info.
        /// </summary>
        /// <param name="cont"></param>
        public static void SaveChanges(Contact cont)
        {
            Entities entities = Common.DataEntities;

            Contact contact = GetContactByContactID(cont.ContactID, entities);
            Clone(cont, contact);
            entities.SaveChanges();
        }

        /// <summary>
        /// Clone the contact info.
        /// </summary>
        /// <param name="desContact"></param>
        /// <param name="tarContact"></param>
        public static void Clone(Contact desContact, Contact tarContact)
        {
            //tarContact.CompanyName = desContact.CompanyName;
            tarContact.EmailAddress = desContact.EmailAddress;
            tarContact.FirstName = desContact.FirstName;
            tarContact.LastName = desContact.LastName;
            tarContact.NameStyle = desContact.NameStyle;
            tarContact.Phone = desContact.Phone;
            //tarContact.SalesPerson = desContact.SalesPerson;
            tarContact.Suffix = desContact.Suffix;
            tarContact.Title = desContact.Title;
        }
    }
}

