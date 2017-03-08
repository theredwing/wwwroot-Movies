using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using EpicAdventureWorks;
using AdventureWorks;
using AdventureWorks.Models;
using AdventureWorksDataModel;
using System.Collections;
using System.Text;
using System.Collections.Specialized;


namespace AdventureWorks.Controllers
{
    public class ProductsController : Controller
    {

        private AdventureWorksDataModel.Product _currentProduct;
        private AdventureWorksDataModel.ProductCategory _currentProductCategory;
        protected AdventureWorksDataModel.ProductSubcategory _currentProductSubcategory;

        /// <summary>
        /// Gets the current product.
        /// </summary>
        /// <value>The current product.</value>
        public AdventureWorksDataModel.Product CurrentProduct
        {
            get
            {
                AdventureWorksDataModel.Product p = null;
                if (_currentProduct != null)
                {
                    p = _currentProduct;
                }
                else
                {
                    var s = "";
                    if (Request.QueryString["Product"] != null)
                    {
                        s = HttpUtility.UrlDecode(Request.QueryString["Product"]);
                        p = Common.DataEntities.Product.First(prod => prod.Name == s);
                        _currentProduct = p;
                    }
                }
                return p;
            }
        }

        /// <summary>
        /// Gets the current product category.
        /// </summary>
        /// <value>The current product category.</value>
        protected AdventureWorksDataModel.ProductCategory CurrentProductCategory
        {
            get
            {
                AdventureWorksDataModel.ProductCategory c;
                if (_currentProductCategory == null)
                {
                    string category = CurrentProductCategoryName;
                    if (!string.IsNullOrEmpty(category))
                    {
                        string s = HttpUtility.UrlDecode(category);
                        c = CategoryManager.GetCategoryByName(s);
                        _currentProductCategory = c;
                    }
                }
                return _currentProductCategory;
            }
        }


        /// <summary>
        /// Gets the current product subcategory.
        /// </summary>
        /// <value>The current product subcategory.</value>
        public AdventureWorksDataModel.ProductSubcategory CurrentProductSubcategory
        {
            get
            {
                AdventureWorksDataModel.ProductSubcategory c = null;
                if (_currentProductSubcategory != null)
                {
                    c = _currentProductSubcategory;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["SubCategory"]))
                    {
                        string s = HttpUtility.UrlDecode(Request.QueryString["SubCategory"]);
                        c = CategoryManager.GetProductSubcategoryByName(s);
                        _currentProductSubcategory = c;
                    }
                }
                return c;
            }
        }

        /// <summary>
        /// Gets the current product category.
        /// </summary>
        /// <value>The current product category.</value>
        public string CurrentProductCategoryName
        {
            get
            {
                return Request.QueryString["Category"];
            }
        }

        /// <summary>
        /// Gets the current sub product category.
        /// </summary>
        /// <value>The current product category.</value>
        public string CurrentSubProductCategoryName
        {
            get
            {
                return Request.QueryString["SubCategory"];
            }
        }

        public JsonResult GetProductRelatedData(string CatStr, string SubCatStr, string ProductStr)
        {
            try
            {
                List<string> RelatedMenuList = new List<string>();
                List<string> ViewedProductMenuList = new List<string>();
                int RelatedMenuListCount = 0, ViewedProductMenuListCount = 0;

                //For Related Products Link Menu                    
                // ramdom products
                IQueryable<Product> categoryList = ProductManager.GetProductByCategory(CategoryManager.GetProductSubcategoryByName((SubCatStr)));
                var items = from ls in categoryList.ToList()
                            select new { ls, GID = Guid.NewGuid() };
                var items1 = from i in items
                             orderby i.GID
                             select i.ls;
                List<Product> productcategoryList = items1.Take(5).ToList();
                foreach (Product item in productcategoryList)
                {
                    // if the categery is current category not show the category product link 
                    if (!(Common.DataEntities.Product.First(prod => prod.Name == ProductStr).ProductID == item.ProductID))
                    {
                        string MenuItem = "<a href='" + string.Format("/Products/Index?Category={0}&SubCategory={1}&Product={2}", HttpUtility.UrlEncode(CatStr), HttpUtility.UrlEncode(SubCatStr), HttpUtility.UrlEncode(item.Name)) + "'>" + item.Name + "</a>";
                        RelatedMenuList.Add(MenuItem);
                        RelatedMenuListCount++;
                    }
                }

                //For Recently viewed Link Menu
                HttpCookie cookie = Request.Cookies["product"];
                if (cookie != null)
                {
                    NameValueCollection cookievalue = cookie.Values;
                    string prodName = null;
                    string category = null;
                    string subcategory = null;
                    int valueCnt = cookievalue.Count;
                    for (int i = (valueCnt - 1); i >= (valueCnt > 5 ? (valueCnt - 5) : 0); i--)
                    {
                        if (!cookievalue.AllKeys[i].Equals(Common.DataEntities.Product.First(prod => prod.Name == ProductStr).ProductID.ToString()))
                        {
                            string[] values = cookievalue[i].Split('_');
                            if (values.Length == 3)
                            {
                                prodName = values[0];
                                category = values[1];
                                subcategory = values[2];
                                string MenuItem = "<a href='" + string.Format("/Products/Index?Category={0}&SubCategory={1}&Product={2}", HttpUtility.UrlEncode(category), HttpUtility.UrlEncode(subcategory), HttpUtility.UrlEncode(prodName)) + "'>" + prodName + "</a>";
                                ViewedProductMenuList.Add(MenuItem);
                                ViewedProductMenuListCount++;
                            }
                        }
                    }
                }
                return Json(new { ResultType = "Success", ErrMsg = "", RelatedMenuList = RelatedMenuList, ViewedProductMenuList = ViewedProductMenuList, RelatedMenuListCount = RelatedMenuListCount, ViewedProductMenuListCount = ViewedProductMenuListCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Set Product's Specs Data
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="panel"></param>
        private string SetSpecs(Product prod)
        {
            StringBuilder sb = new StringBuilder();
            if (prod != null)
            {
                sb.Append("<ul class=\"ProductSpecsContainer\">");
                if (!string.IsNullOrEmpty(prod.Color))
                {
                    sb.Append("<li class=\"ProductSpecsColorLabel\">Color:</li>");
                    sb.AppendFormat("<li class=\"ProductSpecsColorValue\">{0}</li>", prod.Color);
                }
                if (!string.IsNullOrEmpty(prod.Size))
                {
                    sb.Append("<li class=\"ProductSpecsSizeLabel\">Size:</li>");
                    prod.SizeUnitMeasureReference.Load();

                    sb.AppendFormat("<li class=\"ProductSpecsSizeValue\">{0} {1}</li>", prod.Size, prod.SizeUnitMeasure == null ? "" : prod.SizeUnitMeasure.Name);
                }
                if (prod.Weight != null && prod.Weight > 0)
                {
                    sb.Append("<li class=\"ProductSpecsWeightLabel\">Weight:</li>");
                    prod.WeightUnitMeasureReference.Load();
                    sb.AppendFormat("<li class=\"ProductSpecsWeightValue\">{0} {1}</li>", prod.Weight, prod.WeightUnitMeasure == null ? "" : prod.WeightUnitMeasure.Name);
                }
                sb.Append("</ul>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Set Product's Model Data
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="panel"></param>
        private string SetModel(ProductModel model)
        {
            StringBuilder sb = new StringBuilder();
            if (model != null)
            {

                sb.Append("<div class=\"ProductModelContainer\">");
                sb.Append("<h1 class=\"ProductModelSizeLabel\">About</h1>");
                sb.AppendFormat("<p class=\"ProductModelDescriptionValue\">{0}</p>", model.CatalogDescription);
                sb.Append("</div>");
            }
            return sb.ToString();
        }


        /// <summary>
        /// Set Viewed Product Info into cookies
        /// </summary>
        private void SetCookies()
        {
            string currentProdInfo = CurrentProduct.Name + "_" + CurrentProductCategory.Name + "_" + CurrentProductSubcategory.Name;
            string currentProdId = CurrentProduct.ProductID.ToString();
            if (Request.Cookies["product"] == null)
            {
                HttpCookie cookie = new HttpCookie("product");
                cookie.Expires = DateTime.Now.AddDays(1);
                cookie.Values[currentProdId] = currentProdInfo;
                Response.Cookies.Add(cookie);
            }
            else
            {
                HttpCookie cookie = Request.Cookies["product"];
                cookie.Expires = DateTime.Now.AddDays(1);
                NameValueCollection cookieValue = cookie.Values;
                bool isExistInCookie = false;
                for (int i = 0; i < cookieValue.Count; i++)
                {
                    if (cookieValue.AllKeys[i].Equals(currentProdId))
                    {
                        isExistInCookie = true;
                        break;
                    }
                }
                if (!isExistInCookie)
                    cookie.Values[currentProdId] = currentProdInfo;
                Response.Cookies.Add(cookie);
            }
        }

        public ActionResult Index()
        {
            if (CurrentProduct != null)
            {
                Product prod = CurrentProduct;
                if (prod != null)
                {
                    SetCookies();
                    ProductsModel ViewProduct = new ProductsModel();
                    prod.ProductSubcategoryReference.Load();
                    prod.ProductSubcategory.ProductCategoryReference.Load();
                    ViewProduct.PageTitle = "AdventureWorks Cycles - " + prod.Name;
                    ViewBag.BodyClass = prod.ProductSubcategory.Name.Replace(" ", "") + " " + prod.ProductSubcategory.ProductCategory.Name.Replace(" ", "");
                    ViewProduct.ProductID = prod.ProductID.ToString();
                    ViewProduct.ProductName = prod.Name;
                    ViewProduct.ProductPrice = prod.ListPrice.ToString("C");
                    ViewProduct.phSpecsHTML = SetSpecs(prod);
                    prod.ProductModelReference.Load();
                    if (prod.ProductModel != null)
                    {
                        prod.ProductModel.ProductModelProductDescriptionCulture.Load();
                        if (prod.ProductModel.ProductModelProductDescriptionCulture.Count > 0)
                        {
                            ProductModelProductDescriptionCulture desc = prod.ProductModel.ProductModelProductDescriptionCulture.First(pd => pd.CultureID.Trim() == "en");
                            desc.ProductDescriptionReference.Load();
                            ViewProduct.ProductDescVisibility = "visible";
                            ViewProduct.ProductDesc = desc.ProductDescription.Description;
                            ViewProduct.phModelHTML = SetModel(prod.ProductModel);
                        }
                        else
                        {
                            ViewProduct.ProductDescVisibility = "hidden";
                        }

                    }
                    if (CurrentProductCategoryName == "Bikes")
                    {
                        if (CurrentProductSubcategory != null)
                        {
                            ViewProduct.ChartVisibility = "visible";
                            ViewProduct.ChartCategory = CurrentProductSubcategory.Name;
                        }
                    }
                    else
                    {
                        ViewProduct.ChartVisibility = "hidden";
                    }
                    return View("_ViewProduct", "_SiteLayout", ViewProduct);
                }
            }
            if (CurrentProductSubcategory != null)
            {
                ProductsModel ListProducts = new ProductsModel();
                ListProducts.PageTitle = "AdventureWorks Cycles - " + CurrentProductSubcategory.Name;
                ViewBag.BodyClass += CurrentProductCategory.Name.Replace(" ", "") + " " + CurrentProductSubcategory.Name.Replace(" ", "");
                int currentCategoryId = CurrentProductSubcategory.ProductSubcategoryID;
                ListProducts.ProductCategoryId = currentCategoryId;
                ListProducts.ProductColorsList = ProductManager.GetProductColor(currentCategoryId);
                if (ListProducts.ProductColorsList != null && ListProducts.ProductColorsList.Count > 0)
                {
                    ListProducts.ProductColorsList.Insert(0, "All");
                    ListProducts.ColorDivVisibility = "visible";

                }
                else
                {
                    ListProducts.ColorDivVisibility = "hidden";
                }

                ListProducts.ProductWeightsList = ProductManager.GetProductWeightString(currentCategoryId);
                if (ListProducts.ProductWeightsList.Count > 0)
                {
                    ListProducts.ProductWeightsList.Insert(0, "All");
                    ListProducts.WeightDivVisibility = "visible";
                }
                else
                {
                    ListProducts.WeightDivVisibility = "hidden";
                }

                List<string> sizes = ProductManager.GetProductSize(currentCategoryId);
                int sizeCnt = sizes.Count();
                if (sizeCnt > 0)
                {
                    ListProducts.ProductSizeMin = 0;
                    ListProducts.ProductSizeMax = sizeCnt;
                    List<string> sizeList = new List<string>();
                    sizeList.Add("All");
                    int Temp = 0;
                    ListProducts.ProductSizesListArray = new string[100];
                    ListProducts.ProductSizesListArray[Temp] = "All";
                    foreach (string size in sizes)
                    {
                        Temp++;
                        ListProducts.ProductSizesListArray[Temp] = size;
                        sizeList.Add(size);
                    }
                    ListProducts.ProductSizesList = sizeList;
                    ListProducts.SizeDivVisibility = "visible";
                }
                else
                {
                    ListProducts.SizeDivVisibility = "false";
                }
                return View("_ListProducts", "_SiteLayout", ListProducts);
            }
            if (CurrentProductCategory != null)
            {
                ViewBag.PageTitle = "AdventureWorks Cycles - " + CurrentProductCategory.Name;
                string s = HttpUtility.UrlDecode(Request.QueryString["Category"]);
                var SelectedProductCategory = CategoryManager.GetCategoryByName(s);
                SelectedProductCategory.ProductSubcategory.Load();
                List<ProductSubcategory> SubCatList = SelectedProductCategory.ProductSubcategory.ToList();
                ProductsModel ListCategories = new ProductsModel();
                ListCategories.ProductSubCategoryList = SubCatList;
                List<string> CarNameList = new List<string>();
                for (int i = 0; i < SubCatList.Count; i++)
                {
                    CarNameList.Add("C1Carousel" + SubCatList[i].ProductSubcategoryID);
                }
                ListCategories.CarouselNameList = CarNameList;
                ListCategories.ChartVisibility = "hidden";
                if (CurrentProductCategory.Name == "Bikes")
                {
                    ListCategories.ChartVisibility = "visible";
                }
                ViewBag.BodyClass += CurrentProductCategory.Name.Replace(" ", "");
                return View("_ListCategories", "_SiteLayout", ListCategories);
            }
            Response.Redirect("~/Home/Default");
            return View();
        }

        public JsonResult GetProductWeightSizeLists(string ProductCategoryId)
        {
            try
            {
                if (int.Parse(ProductCategoryId) > 0)
                {
                    List<decimal?> WeightsList = ProductManager.GetProductWeight(int.Parse(ProductCategoryId));
                    List<string> SizesList = ProductManager.GetProductSize(int.Parse(ProductCategoryId));
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", WeightsList = WeightsList, SizesList = SizesList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ResultType = "Success", ErrMsg = "No Product is selected to Add." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddToCart(int ProductID)
        {
            try
            {
                if (ProductID > 0)
                {
                    ShoppingCartManager.AddToCart(ProductID, 1);
                    return Json(new { ResultType = "Success", ErrMsg = "N/A" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ResultType = "Success", ErrMsg = "No Product is selected to Add." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMenus(string CurrentCategoryName, string CurrentSubCategory)
        {
            try
            {
                string[] MenuList = new string[100];
                string[] SubMenuList = new string[100];
                List<ProductCategory> mainCatetories = CategoryManager.GetMainCategories();
                int MainMenuIndex = 0, SubMenuItemsCount = 0;
                foreach (ProductCategory pc in mainCatetories)
                {
                    MenuList[MainMenuIndex] = "<a href='/Products/Index?Category=" + pc.Name + "'>" + pc.Name + "</a>";
                    if (pc.Name == CurrentCategoryName)
                    {
                        MenuList[MainMenuIndex] = "<a href='/Products/Index?Category=" + pc.Name + "' class='currentmenu'>" + pc.Name + "</a>";

                        //load sub categories
                        pc.ProductSubcategory.Load();
                        int SubMenuIndex = 0;
                        foreach (ProductSubcategory productSubcategory in pc.ProductSubcategory)
                        {
                            SubMenuList[SubMenuIndex] = "<a href='/Products/Index?Category=" + pc.Name + "&SubCategory=" + productSubcategory.Name + "' class=" + productSubcategory.Name.Replace(" ", "") + ">" + productSubcategory.Name + "</a>";
                            if (CurrentSubCategory != null && productSubcategory.Name == CurrentSubCategory)
                            {
                                SubMenuList[SubMenuIndex] = "<a href='/Products/Index?Category=" + pc.Name + "&SubCategory=" + productSubcategory.Name + "' class='currentsubmenu'>" + productSubcategory.Name + "</a>";
                            }
                            SubMenuItemsCount++;
                            SubMenuIndex++;
                        }
                    }
                    MainMenuIndex++;
                }
                return Json(new { ResultType = "Success", ErrMsg = "", MenuList = MenuList, SubMenuList = SubMenuList, MenuListItemsCount = MainMenuIndex, SubMenuListItemsCount = SubMenuItemsCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message, MenuList = "", SubMenuList = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetListCategoriesCarouselData(string CatStr)
        {
            try
            {
                var SelectedProductCategory = CategoryManager.GetCategoryByName(CatStr.Trim());
                SelectedProductCategory.ProductSubcategory.Load();
                List<ProductSubcategory> ProductSubCategoryList = SelectedProductCategory.ProductSubcategory.ToList();
                List<string> ProductSubCategoryIDList = new List<string>();
                List<string> ProductSubCategoryNameList = new List<string>();
                List<List<string>> CarouselDataList = new List<List<string>>();
                for (int i = 0; i < SelectedProductCategory.ProductSubcategory.Count; i++)
                {
                    ProductSubCategoryIDList.Add(SelectedProductCategory.ProductSubcategory.ToList()[i].ProductSubcategoryID.ToString());
                    ProductSubCategoryNameList.Add(SelectedProductCategory.ProductSubcategory.ToList()[i].Name);
                    CarouselDataList.Add(GetCarouselString(CatStr, SelectedProductCategory.ProductSubcategory.ToList()[i].Name));
                }
                return Json(new { ResultType = "Success", ErrMsg = "N/A", ProductSubCategoryIDList = ProductSubCategoryIDList, ProductSubCategoryNameList = ProductSubCategoryNameList, CarouselDataList = CarouselDataList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }

        private List<string> GetCarouselString(string CatStr, string SubCatStr)
        {
            ProductSubcategory subCat = (ProductSubcategory)CategoryManager.GetProductSubcategoryByName(SubCatStr);
            List<string> RetValue = new List<string>();
            IQueryable<Product> prods = ProductManager.GetProductByCategory(subCat);
            if (prods.Count() > 0)
            {
                Table item = new Table();
                TableRow itemrow = new TableRow();
                int index = 0;
                foreach (Product prod in prods)
                {
                    // Multipage,s PageView add 
                    if (index % 7 == 0)
                    {
                        item = new Table();
                        itemrow = new TableRow();
                    }
                    index++;
                    HtmlGenericControl link = new HtmlGenericControl("a");
                    link.Attributes.Add("href", "../Products/Index?Category=" + CatStr + "&SubCategory=" + HttpUtility.UrlEncode(subCat.Name) + "&Product=" + HttpUtility.UrlEncode(prod.Name));
                    HtmlGenericControl img = new HtmlGenericControl("img");
                    img.Attributes.Add("class", "SubCategoryChart");
                    img.Attributes.Add("alt", prod.Name);
                    link.Attributes.Add("id", prod.ProductID.ToString());
                    img.Attributes.Add("src", "../ProductImage.ashx?ProductID=" + HttpUtility.UrlEncode(prod.ProductID.ToString()));
                    link.Controls.Add(img);
                    TableCell itemcell = new TableCell();
                    itemcell.Controls.Add(link);
                    itemrow.Cells.Add(itemcell);
                    if (index % 7 == 0)
                    {
                        item.Rows.Add(itemrow);
                        TextWriter tw = new StringWriter();
                        HtmlTextWriter hw = new HtmlTextWriter(tw);
                        item.RenderControl(hw);
                        RetValue.Add(tw.ToString());
                    }
                    else if (index == prods.Count() && index % 7 != 0)
                    {
                        item.Rows.Add(itemrow);
                        TextWriter tw = new StringWriter();
                        HtmlTextWriter hw = new HtmlTextWriter(tw);
                        item.RenderControl(hw);
                        RetValue.Add(tw.ToString());
                    }
                }
            }
            return RetValue;
        }

        public JsonResult GetToolTipData(string ProductId)
        {
            try
            {
                Product prod = ProductManager.GetProductByProductId(int.Parse(ProductId));
                if (prod != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class=\"ToolTipTable\">");
                    sb.AppendFormat("<div class=\"ToolTipLabelName\">{0}</div>", prod.Name);
                    sb.AppendFormat("<div class=\"ToolTipLabelPrice\">{0}</div>", prod.ListPrice.ToString("C"));
                    sb.Append("<ul class=\"description\" >");
                    sb.Append("<li colspan=\"2\" class=\"ToolTipLabelSpecs\">Specs</li>");
                    sb.Append("<li class=\"ToolTipLabelColor\">Color:</li>");
                    sb.AppendFormat("<li class=\"ToolTipLabelColorValue\">{0}</li>", prod.Color);
                    sb.Append("<li class=\"ToolTipLabelSize\">Size:</li>");
                    sb.AppendFormat("<li class=\"ToolTipLabelSizeValue\">{0}</li>", prod.Size);
                    sb.Append("<li class=\"ToolTipLabelWeight\">Weight:</li>");
                    sb.AppendFormat("<li class=\"ToolTipLabelWeightValue\">{0}Kg</li>", prod.Weight);
                    sb.Append("</ul>");
                    sb.Append("</div>");
                    return Json(new { ResultType = "Success", ErrMsg = "N/A", ToolTipData = sb.ToString() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { ResultType = "Success", ErrMsg = "Invalid Product.", }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetListProductsCarouselData(string CatStr, string SubCatStr, string Color, string Size, string Weight)
        {
            List<string> CarouselDataList = new List<string>();
            int PageCount = 0;
            try
            {
                IQueryable<Product> prods = ProductManager.GetProductByCategory(CategoryManager.GetProductSubcategoryByName(SubCatStr));
                if (Color != "")
                {
                    string currentColor = Color;
                    prods = prods.Where(p => p.Color == currentColor);
                }
                if (Weight != "")
                {
                    decimal currentWeight = decimal.Parse(Weight);
                    prods = prods.Where(p => p.Weight == currentWeight);
                }
                if (Size != "")
                {
                    string currentSize = Size;
                    prods = prods.Where(p => p.Size == currentSize);
                }
                if (prods.Count() > 0)
                {
                    int index = 0;
                    int rowIndex = 0;
                    HtmlGenericControl item = new HtmlGenericControl("div");
                    Table table = null;
                    TableRow tRow = null;
                    foreach (Product prod in prods)
                    {
                        // Multipage,s PageView add 
                        if (rowIndex % 4 == 0)
                        {
                            item = new HtmlGenericControl("div");
                            table = new Table();
                            table.Style.Add(HtmlTextWriterStyle.Width, "100%");
                        }
                        if (index % 4 == 0)
                        {
                            tRow = new TableRow();
                            tRow.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                        }
                        index++;
                        HtmlGenericControl link = new HtmlGenericControl("a");
                        link.Attributes.Add("href", "/Products/Index?Category=" + HttpUtility.UrlEncode(CatStr) + "&SubCategory=" + HttpUtility.UrlEncode(SubCatStr) + "&Product=" + HttpUtility.UrlEncode(prod.Name));
                        link.Attributes.Add("class", "ListProductsImage");

                        HtmlGenericControl img = new HtmlGenericControl("img");
                        img.Attributes.Add("alt", prod.Name);
                        link.Attributes.Add("id", prod.ProductID.ToString());
                        img.Attributes.Add("src", "/ProductImage.ashx?ProductID=" + HttpUtility.UrlEncode(prod.ProductID.ToString()));
                        link.Controls.Add(img);

                        HtmlGenericControl div = new HtmlGenericControl("div");
                        div.InnerText = prod.Name;
                        div.Attributes.Add("class", "ListProductHeader");
                        TableCell tCell = new TableCell();
                        tCell.Attributes.Add("class", "ListProductBlock");
                        tCell.Controls.Add(link);
                        tCell.Controls.Add(div);
                        tRow.Cells.Add(tCell);

                        if (index % 4 == 0)
                        {
                            table.Rows.Add(tRow);
                            rowIndex++;
                        }
                        else if (index == prods.Count() && (index % 4) != 0)
                        {
                            table.Rows.Add(tRow);
                        }

                        if ((rowIndex % 4 == 0) && (index % 4 == 0))
                        {
                            item.Controls.Add(table);
                            TextWriter tw = new StringWriter();
                            HtmlTextWriter hw = new HtmlTextWriter(tw);
                            item.RenderControl(hw);
                            CarouselDataList.Add(tw.ToString());
                            PageCount++;
                        }
                        else if (index == prods.Count() && (index + 1) % 16 != 0)
                        {
                            item.Controls.Add(table);
                            TextWriter tw = new StringWriter();
                            HtmlTextWriter hw = new HtmlTextWriter(tw);
                            item.RenderControl(hw);
                            CarouselDataList.Add(tw.ToString());
                            PageCount++;
                        }
                    }
                }
                return Json(new { ResultType = "Success", ErrMsg = "", CarouselData = CarouselDataList, PageCount = PageCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ResultType = "Fail", ErrMsg = "Error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}
