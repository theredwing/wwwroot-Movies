using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public partial class MovieTitleV
    {
        public MovieTitleV() { }
        public Movie Movies { get; set; }
        public int MovieID { get; set; }
        public string strMovieTitle { get; set; }
        public string strDescription { get; set; }
        public MovieTitleV(Movie MovieTitles)
        {
            Movies = MovieTitles;
            strMovieTitle = "";
            strDescription = "";
            MovieID = 0;
        }
    }
}