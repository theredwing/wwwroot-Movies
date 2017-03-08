using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdventureWorksDataModel;

namespace AdventureWorks.Models
{
    public class SiteLayoutModel
    {
        public string AnonymousTemplateVisibility { get; set; }
        public string LoggedInTemplateVisibility { get; set; }
        public string LoggedInEmailID { get; set; }
        public string ShoppingCartItemsCount { get; set; }
        public string SiteMapHTML { get; set; }        
    }
    
}