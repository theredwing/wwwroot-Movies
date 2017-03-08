using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdventureWorksDataModel;
using EpicAdventureWorks;

namespace AdventureWorks
{
	/// <summary>
	/// Summary description for ProductImage
	/// </summary>
	public class ProductImage : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			int prodId;
			int.TryParse(context.Request.QueryString["ProductID"], out prodId);
			byte[] img;
			if (prodId > 0)
			{
				Product p = ProductManager.GetProductByProductId(prodId);
				p.ProductProductPhoto.Load();
				ProductProductPhoto photo = p.ProductProductPhoto.FirstOrDefault();
				if (photo != null)
				{
					photo.ProductPhotoReference.Load();
					if (context.Request.QueryString["size"] != null && context.Request.QueryString["size"] == "large")
					{
						img = photo.ProductPhoto.LargePhoto;
					}
					else
					{
						img = photo.ProductPhoto.ThumbNailPhoto;
					}
					context.Response.Cache.SetExpires(DateTime.Today.AddMonths(3));
					context.Response.Cache.SetCacheability(HttpCacheability.Public);
					context.Response.Cache.SetValidUntilExpires(true);
					context.Response.ContentType = "image/jpeg";
					context.Response.BinaryWrite(img);
					context.Response.End();
				}
				else
				{
					context.Response.Write("No Image Availabe");
					context.Response.End();
				}
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}