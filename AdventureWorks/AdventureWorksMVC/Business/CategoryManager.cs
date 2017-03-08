using System;
using System.Collections.Generic;
using System.Linq;
using AdventureWorksDataModel;

namespace EpicAdventureWorks
{
    /// <summary>
    /// Contains methods with related to shopping cart. 
    /// </summary>
    public class CategoryManager
    {
        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static List<ProductCategory> GetMainCategories()
        {
           var cats = from cat in Common.DataEntities.ProductCategory
                       select cat;
           return cats.ToList();
        }

        /// <summary>
        /// Gets the name of the category by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ProductCategory GetCategoryByName(string name)
        {
            var cats = from cat in Common.DataEntities.ProductCategory
                       where cat.Name == name
                       select cat;
            return cats.FirstOrDefault();
        }

        /// <summary>
        /// Gets the name of the category by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ProductSubcategory GetProductSubcategoryByName(string name)
        {
            var cats = from cat in Common.DataEntities.ProductSubcategory
                       where cat.Name == name
                       select cat;
            return cats.FirstOrDefault();
        }

    }
}