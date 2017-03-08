using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Movies.Models
{
    public class MovieInitializer : CreateDatabaseIfNotExists<MovieDBContext>
    {
        protected override void Seed(MovieDBContext context)
        {
            base.Seed(context);
        }
    }
}