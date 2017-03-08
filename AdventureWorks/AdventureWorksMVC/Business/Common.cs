using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using AdventureWorksDataModel;

namespace EpicAdventureWorks
{
    /// <summary>
    /// Contains common helper methods and properties used in this application.
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Gets the data entities.
        /// </summary>
        /// <value>The data entities.</value>
        public static Entities DataEntities
        {
            get
            {
                return new Entities();
            }
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageURL()
        {
            return GetLoginPageURL(string.Empty);
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="ReturnUrl">Return url</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageURL(string returnUrl)
        {
            string redirectUrl;
            if (!string.IsNullOrEmpty(returnUrl))
            {
                redirectUrl = string.Format(CultureInfo.InvariantCulture, "~/Login.aspx?ReturnUrl={0}", HttpUtility.UrlEncode(returnUrl));
            }
            else
            {
                redirectUrl = string.Format(CultureInfo.InvariantCulture, "~/Login.aspx");
            }
            return redirectUrl;
        }

        /// <summary>
        /// Gets login page URL
        /// </summary>
        /// <param name="AddCurrentPageURL">A value indicating whether add current page url as "ReturnURL" parameter</param>
        /// <param name="CheckoutAsGuest">A value indicating whether login page will show "Checkout as a guest or Register" message</param>
        /// <returns>Login page URL</returns>
        public static string GetLoginPageURL(bool AddCurrentPageURL, bool CheckoutAsGuest)
        {
            string redirectUrl = string.Empty;
            if (AddCurrentPageURL)
            {
                redirectUrl = string.Format(CultureInfo.InvariantCulture, "~/Login.aspx?ReturnUrl={0}", HttpUtility.UrlEncode(HttpContext.Current.Request.RawUrl));
            }
            else
            {
                redirectUrl = GetLoginPageURL();
            }

            if (CheckoutAsGuest)
            {
                redirectUrl = ModifyQueryString(redirectUrl, "CheckoutAsGuest=true", string.Empty);
            }
            return redirectUrl;
        }

        /// <summary>
        /// Modifies query string
        /// </summary>
        /// <param name="url">Url to modify</param>
        /// <param name="queryStringModification">Query string modification</param>
        /// <param name="targetLocationModification">Target location modification</param>
        /// <returns>New url</returns>
        public static string ModifyQueryString(string url, string queryStringModification, string targetLocationModification)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            if (url.Contains("#"))
            {
                str2 = url.Substring(url.IndexOf("#") + 1);
                url = url.Substring(0, url.IndexOf("#"));
            }
            if (url.Contains("?"))
            {
                str = url.Substring(url.IndexOf("?") + 1);
                url = url.Substring(0, url.IndexOf("?"));
            }
            if (!string.IsNullOrEmpty(queryStringModification))
            {
                if (!string.IsNullOrEmpty(str))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str3 in str.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str3))
                        {
                            string[] strArray = str3.Split(new char[] { '=' });
                            if (strArray.Length == 2)
                            {
                                dictionary[strArray[0]] = strArray[1];
                            }
                            else
                            {
                                dictionary[str3] = null;
                            }
                        }
                    }
                    foreach (string str4 in queryStringModification.Split(new char[] { '&' }))
                    {
                        if (!string.IsNullOrEmpty(str4))
                        {
                            string[] strArray2 = str4.Split(new char[] { '=' });
                            if (strArray2.Length == 2)
                            {
                                dictionary[strArray2[0]] = strArray2[1];
                            }
                            else
                            {
                                dictionary[str4] = null;
                            }
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    foreach (string str5 in dictionary.Keys)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append("&");
                        }
                        builder.Append(str5);
                        if (dictionary[str5] != null)
                        {
                            builder.Append("=");
                            builder.Append(dictionary[str5]);
                        }
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = queryStringModification;
                }
            }
            if (!string.IsNullOrEmpty(targetLocationModification))
            {
                str2 = targetLocationModification;
            }
            return (url + (string.IsNullOrEmpty(str) ? "" : ("?" + str)) + (string.IsNullOrEmpty(str2) ? "" : ("#" + str2)));
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="key">The key.</param>
        /// <param name="val">The val.</param>
        public static void SetCookie(HttpApplication application, string key, string val)
        {
            HttpCookie cookie = new HttpCookie(key);
            cookie.Value = val;
            if (string.IsNullOrEmpty(val))
            {
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }
            else
            {
                cookie.Expires = DateTime.Now.AddMonths(1);
            }
            application.Response.Cookies.Remove(key);
            application.Response.Cookies.Add(cookie);
        }

        internal static void SetCookie(HttpCookie k)
        {
            SetCookie(HttpContext.Current.ApplicationInstance, k.Name, k.Value);
        }
    }
}