using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdventureWorksDataModel;
using EpicAdventureWorks;
using AdventureWorks.Models;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Web.UI;



namespace AdventureWorks.Controllers
{
    public class SiteLayoutController : Controller
    {
        public ActionResult HeaderLayout()
        {
            SiteLayoutModel Header = new SiteLayoutModel();
            if (User.Identity.IsAuthenticated)
            {
                Header.AnonymousTemplateVisibility = "hidden";
                Header.LoggedInTemplateVisibility = "visible";
            }
            else
            {
                Header.AnonymousTemplateVisibility = "visible";
                Header.LoggedInTemplateVisibility = "hidden";
            }
            Header.LoggedInEmailID = User.Identity.Name;
            Header.ShoppingCartItemsCount = RequestContext.Current.UserShoppingCart.ShoppingCarItems.Count.ToString();
            return View(Header);
        }

        public ActionResult FooterLayout()
        {
            SiteLayoutModel Footer = new SiteLayoutModel();
            Footer.SiteMapHTML = LoadSiteMap();
            return View(Footer);
        }

        private string LoadSiteMap()
        {
            string RetValue = "";
            try
            {
                List<ProductCategory> mainCatetories = CategoryManager.GetMainCategories();
                foreach (ProductCategory category in mainCatetories)
                {
                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes.Add("class", category.Name + "Menu");
                    HtmlGenericControl h1 = new HtmlGenericControl("h1");
                    HtmlAnchor mainLink = new HtmlAnchor();
                    mainLink.InnerText = category.Name;
                    mainLink.HRef = "/Products/Index?Category=" + category.Name;
                    div.Controls.Add(h1);
                    h1.Controls.Add(mainLink);
                    category.ProductSubcategory.Load();
                    HtmlGenericControl ul = new HtmlGenericControl("ul");
                    ul.Attributes.Add("class", category.Name + "List");
                    div.Controls.Add(ul);
                    foreach (ProductSubcategory psub in category.ProductSubcategory)
                    {
                        HtmlGenericControl li = new HtmlGenericControl("li");
                        HtmlAnchor link = new HtmlAnchor();
                        link.InnerText = psub.Name;
                        link.HRef = string.Format("/Products/Index?Category={0}&SubCategory={1}", category.Name, psub.Name);
                        li.Controls.Add(link);
                        ul.Controls.Add(li);
                    }
                    TextWriter tw = new StringWriter();
                    HtmlTextWriter hw = new HtmlTextWriter(tw);
                    div.RenderControl(hw);
                    RetValue = RetValue + tw.ToString();
                }
            }
            catch (Exception ex)
            {
                RetValue = "Error occurred: " + ex.Message;
            }
            return RetValue;
        }

    }
}
