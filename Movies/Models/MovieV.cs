using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace Movies.Models
{
    public class MovieV
    {
        public MovieV() { }

        public NamesLU Names { get; set; }
        public PositionsLU Positions { get; private set; }
        public CategoryLU Categories { get; private set; }
        public Movie Movies { get; set; }
        public MovieCategory MovieCategory { get; set; }
        public MovieNamesPosition MovieNamesPosition { get; private set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Category { get; set; }
        public string strMovieTitle { get; set; }
        public string strActorName { get; set; }
        public string strCrewName { get; set; }
        public string strCrewTitle { get; set; }
        public string strCategory { get; set; }
        public string strDescription { get; set; }
        public int intCrewTitleID { get; set; }
        public int intCategoryID { get; set; }
        public int MovieID { get; set; }


        public MovieV(Movie MovieTitles)
        {
            Movies = MovieTitles;
            Names = new NamesLU();
            Positions = new PositionsLU();
            Categories = new CategoryLU();
            Name = "";
            Position = "";
            Category = "";
            strMovieTitle = null;
            strActorName = null;
            strCrewName = null;
            strCrewTitle = "";
            strDescription = "";
            strCategory = "";
            intCrewTitleID = 0;
            intCategoryID = 0;
            MovieID = 0;
        }
    }
}