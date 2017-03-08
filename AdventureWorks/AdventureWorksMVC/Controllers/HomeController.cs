using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdventureWorksDataModel;
using EpicAdventureWorks;
using AdventureWorks;
using System.Web.Security;
using System.IO;
using System.Reflection;
using System.Text;
 
namespace AdventureWorks.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Default()
        {
            ViewBag.BodyClass = "homepage";
            return View();
        }

        public ActionResult Login()
        {
            string ReturnURL = HttpContext.Request.UrlReferrer.ToString();
            if (ReturnURL.Contains("Home/Login"))
            {
                if (Request.QueryString["RetURL"] != null)
                {
                    if (Request.QueryString["RetURL"].Length > 0)
                    {
                        ReturnURL = Request.QueryString["RetURL"].ToString();
                        if (Request.QueryString["SubCategory"] != null)
                        {
                            ReturnURL += "&SubCategory=" + Request.QueryString["SubCategory"].ToString();
                        }
                        if (Request.QueryString["Product"] != null)
                        {
                            ReturnURL += "&Product=" + Request.QueryString["Product"].ToString();
                        }
                    }
                }
            }
            ViewBag.ReturnURL = ReturnURL;
            if (User.Identity.IsAuthenticated)
            {
                if (ReturnURL.Contains("Home/Login"))
                {
                    Response.Redirect("/Home/Default");
                }
                else
                {
                    Response.Redirect(ReturnURL);
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            string ReturnURL = HttpContext.Request.UrlReferrer.ToString();
            try
            {
                Response.CacheControl = "private";
                Response.Expires = 0;
                Response.AddHeader("pragma", "no-cache");
                CustomerManager.Logout();
                Response.Redirect("/Home/Default");
            }
            catch (Exception ex)
            {
                Response.Redirect(ReturnURL);
            }
            return View();
        }

        public JsonResult UserLogin(string UserName, string Password, string RememberMe)
        {
            // only user name is valdiated.
            try
            {
                if (CustomerManager.Login(UserName.Trim(), Password.Trim()))
                {
                    //if (Membership.ValidateUser(UserName.Trim(), Password.Trim()))
                    //{
                    //    FormsAuthentication.RedirectFromLoginPage(UserName.Trim(), false);
                    //    return Json(new { ResultType = "Success", ErrMsg = "N/A" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { ResultType = "Success", ErrMsg = "Email or Password is invalid." }, JsonRequestBehavior.AllowGet);
                    //}
                    FormsAuthentication.RedirectFromLoginPage(UserName.Trim(), false);
                    return Json(new { ResultType = "Success", ErrMsg = "N/A" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ResultType = "Success", ErrMsg = "Email or Password is invalid." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SignUp(string UserEmail, string Password, string FirstName, string LastName)
        {
            try
            {
                if (Membership.GetUser(UserEmail.Trim()) == null && ContactManager.GetContactByEmail(UserEmail.Trim()) == null)
                {
                    CustomerManager.AddToCustomer(UserEmail.Trim(), Password.Trim(), UserEmail.Trim(), FirstName.Trim(), LastName.Trim());
                    FormsAuthentication.RedirectFromLoginPage(UserEmail.Trim(), true);
                    return Json(new { ResultType = "Success", ErrMsg = "N/A" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ResultType = "Success", ErrMsg = "User exists." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckLoginStatus()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", LoginStatus = "Yes" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", LoginStatus = "No" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message, LoginStatus = "No" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
