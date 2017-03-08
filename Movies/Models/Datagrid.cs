using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace Movies.Models
{
    public class Datagrid
    {
        public string MovieTitle { get; set; }
        public int MovieID { get; set; }
        public string Description { get; set; }
        //public string strMovieTitle { get; set; }
        //public string strActorName { get; set; }
        //public string strCrewName { get; set; }
        //public int intCrewTitleID { get; set; }
        //public virtual Movie Movie { get; set; }
        public List<Movie> MovieTitles;

        public IEnumerator<Movie> GetEnumerator()
        {
            return MovieTitles.GetEnumerator();
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return MovieTitles.GetEnumerator();
        //}
        public Datagrid()
        {
            MovieTitle = "";
            MovieID = 0;
            Description = "";
        }
    }

}