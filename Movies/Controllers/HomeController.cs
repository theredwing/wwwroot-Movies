using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using Movies.Models;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;

namespace Movies.Controllers
{
    public struct MovieStruct
    {
        public string Title;
        public string Actors;
        public string Crew;
        public string Description;
    }

    [HandleError(View="Error")]
    public class HomeController : Controller
    {
        private const int SQLConnection = 2;
        private static Movie mv = new Movie();
        private static MovieCategory mvCat = new MovieCategory();
        private static MovieNamesPosition mvNamesPosition = new MovieNamesPosition();
        private MovieDBContext db = new MovieDBContext();
        private MovieV mvV = new MovieV(mv);
        IList<MovieV> listmvV = new List<MovieV>();
        IList<Datagrid> dg = new List<Datagrid>();
        private IEnumerable<CategoryLU> Categorys = new List<CategoryLU>();
        private IEnumerable<NamesLU> NamesLU = new List<NamesLU>();
        private IEnumerable<PositionsLU> Positions = new List<PositionsLU>();
        private static SortingPagingInfo info = new SortingPagingInfo();
        public MovieStruct[] arrMovies;
        const int MOVIE_CNT = 2000;

        public ActionResult Index()
        {
            //var model = db.MovieV;
            return View();
        }

        public ActionResult MovieSrchAndEdit()
        {
            ViewBag.CrewTitle = "";
            ViewBag.Category = "";
            fillPositionDdl();
            return View();
        }

        public ActionResult MovieImport()
        {
            string strLine;
            string[] arrMovie;
            int i = 0;
            int intMovieRecs = 0;
            ArrayList arrMovieTitles = new ArrayList();
            ArrayList arrMovieActors = new ArrayList();
            ArrayList arrMovieCrew = new ArrayList();
            ArrayList arrMovieDesc = new ArrayList();
            DataTable tblMovieTitle = new DataTable();

            arrMovies = new MovieStruct[MOVIE_CNT];
            arrMovie = new string[5];

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/Movies.csv")))
                {
                    strLine = sr.ReadLine();

                    while (strLine != null)
                    {
                        arrMovie = strLine.Split(new[] { ',' });
                        arrMovies[i].Title = arrMovie[0];
                        arrMovies[i].Actors = arrMovie[1];
                        arrMovies[i].Crew = arrMovie[2];
                        arrMovies[i].Description = arrMovie[3];
                        i++;
                        strLine = sr.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["Message"] = "ERROR in Home.MovieImport: " + e.Message;
                return View("Error");
            }

            DataColumn colMovieID = new DataColumn("MovieID", typeof(int));
            tblMovieTitle.Columns.Add(colMovieID);
            DataColumn colMovieTitle = new DataColumn("MovieTitle", typeof(string));
            tblMovieTitle.Columns.Add(colMovieTitle);

            for (i = 0; i < MOVIE_CNT; i++)
                if (("" + arrMovies[i].Title) != "")
                {
                    arrMovieTitles.Add(arrMovies[i].Title);
                    DataRow row = tblMovieTitle.NewRow();
                    row[tblMovieTitle.Columns[0]] = i;
                    row[tblMovieTitle.Columns[1]] = arrMovies[i].Title;
                    tblMovieTitle.Rows.Add(row);
                }
                else
                    break;

            intMovieRecs = i;
            DataTable tblActors = new DataTable();
            DataColumn colActorID = new DataColumn("ActorID", typeof(int));
            tblActors.Columns.Add(colActorID);
            DataColumn colActorName = new DataColumn("ActorName", typeof(string));
            tblActors.Columns.Add(colActorName);

            for (i = 0; i < MOVIE_CNT; i++)
            {
                arrMovieActors.Add("" + arrMovies[i].Actors);
                DataRow row = tblActors.NewRow();
                row[tblActors.Columns[0]] = i;
                row[tblActors.Columns[1]] = "" + arrMovies[i].Actors;
                tblActors.Rows.Add(row);
            }

            DataTable tblCrew = new DataTable();
            DataColumn colCrewID = new DataColumn("CrewID", typeof(int));
            tblCrew.Columns.Add(colCrewID);
            DataColumn colCrewName = new DataColumn("CrewName", typeof(string));
            tblCrew.Columns.Add(colCrewName);

            for (i = 0; i < MOVIE_CNT; i++)
            {
                arrMovieCrew.Add("" + arrMovies[i].Crew);
                DataRow row = tblCrew.NewRow();
                row[tblCrew.Columns[0]] = i;
                row[tblCrew.Columns[1]] = "" + arrMovies[i].Crew;
                tblCrew.Rows.Add(row);
            }

            DataTable tblDesc = new DataTable();
            DataColumn colDescID = new DataColumn("DescID", typeof(int));
            tblDesc.Columns.Add(colDescID);
            DataColumn colDesc = new DataColumn("Description", typeof(string));
            tblDesc.Columns.Add(colDesc);

            for (i = 0; i < MOVIE_CNT; i++)
            {
                arrMovieDesc.Add("" + arrMovies[i].Description);
                DataRow row = tblDesc.NewRow();
                row[tblDesc.Columns[0]] = i;
                row[tblDesc.Columns[1]] = "" + arrMovies[i].Description;
                tblDesc.Rows.Add(row);
            }

            for (i = 0; i < tblMovieTitle.Rows.Count; i++)
            {
                ParseAndTransfer(tblMovieTitle, tblActors, tblCrew, tblDesc, i);
            }

            ViewData["Message"] = "Import Complete.  Records Processed: " + intMovieRecs.ToString();
            ViewData["Movies"] = arrMovies;
            ViewData["MoviesDB"] = tblMovieTitle;
            return View();
        }

        private void fillPositionDdl()
        {
            Positions = GetPositions().ToList();
            ViewBag.intCrewTitleID = new SelectList(Positions, "PositionsLUID", "Position");
        }

        private IEnumerable<PositionsLU> GetPositions()
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.CommandText = "Select PositionsLUID, Position From PositionsLU Where PositionsLUID <> 4 Order by Position";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return new PositionsLU
                        {
                            PositionsLUID = reader.GetInt32(reader.GetOrdinal("PositionsLUID")),
                            Position = reader.GetString(reader.GetOrdinal("Position"))
                        };
                    }

                    reader.Close();
                }

                cmd.Dispose();
                conn.Close();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Search()
        {
            MovieV mv = new MovieV();
            return View(mv);
        }

        [HttpPost]
        public ActionResult Search(MovieV mvV)
        {
            bool blnFirstTime = true;

            var qryMovieList = db.Database.SqlQuery<Movie>(
                "exec dbo.[spSearchMovies] @strMovieTitle, @strActorName, @strCrewName, @intCrewTitleID",
                    new SqlParameter("strMovieTitle", (object)mvV.strMovieTitle ?? DBNull.Value),
                    new SqlParameter("strActorName", (object)mvV.strActorName ?? DBNull.Value),
                    new SqlParameter("strCrewName", (object)mvV.strCrewName ?? DBNull.Value),
                    new SqlParameter("intCrewTitleID", mvV.intCrewTitleID)
            ).ToList();

            ViewBag.Movie = mv;
            info.PageSize = 10;
            info.SortDirection = "ascending";
            info.SortField = "MovieTitle";
            info.PageCount = GetPgCnt(qryMovieList.Count(), info.PageSize);
            ViewBag.SortingPagingInfo = info;
            ViewBag.MovieSrchVars = mvV;
            db.Database.Connection.Close();

            foreach (var mv in qryMovieList)
            {
                if (blnFirstTime)
                {
                    dg.Add(new Datagrid() { MovieTitles = qryMovieList, MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
                    blnFirstTime = false;
                }
                else
                    dg.Add(new Datagrid() { MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
            }

            dg = dg.Take(info.PageSize).ToList();
            Session["SortDirection"] = info.SortDirection;
            Session["CurrentPageIndex"] = info.CurrentPageIndex;
            return View("Datagrid", dg);
        }

        public ActionResult Datagrid(MovieV mvV, SortingPagingInfo info)
        {
            bool blnFirstTime = true;

            var qryMovieList = db.Database.SqlQuery<Movie>(
                "exec dbo.[spSearchMovies] @strMovieTitle, @strActorName, @strCrewName, @intCrewTitleID",
                    new SqlParameter("strMovieTitle", (object)mvV.strMovieTitle ?? DBNull.Value),
                    new SqlParameter("strActorName", (object)mvV.strActorName ?? DBNull.Value),
                    new SqlParameter("strCrewName", (object)mvV.strCrewName ?? DBNull.Value),
                    new SqlParameter("intCrewTitleID", mvV.intCrewTitleID)
            ).ToList();

            ViewBag.Movie = mv;
            ViewBag.MovieSrchVars = mvV;
            info.PageSize = 10;
            var qryMovieListOrder = new List<Movie>();

            if (info.SortDirection == "ascending")
            {
                qryMovieListOrder = qryMovieList.OrderBy(mv => mv.MovieTitle).ToList();
                Session["SortDirection"] = "ascending";
            }
            else
            {
                qryMovieListOrder = qryMovieList.OrderByDescending(mv => mv.MovieTitle).ToList();
                Session["SortDirection"] = "descending";
            }

            info.SortField = "MovieTitle";
            info.PageCount = GetPgCnt(qryMovieList.Count(), info.PageSize);

            ViewBag.SortingPagingInfo = info;
            db.Database.Connection.Close();
            
            foreach (var mv in qryMovieListOrder)
            {
                if (blnFirstTime)
                {
                    dg.Add(new Datagrid() { MovieTitles = qryMovieList, MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
                    blnFirstTime = false;
                }
                else
                    dg.Add(new Datagrid() { MovieID = mv.MovieID, MovieTitle = mv.MovieTitle, Description = mv.Description });
            }

            dg = dg.Skip(Convert.ToInt32(info.CurrentPageIndex) * info.PageSize).Take(info.PageSize).ToList();
            Session["CurrentPageIndex"] = info.CurrentPageIndex;
            return View("Datagrid", dg);
        }

        private int GetPgCnt(int intMovieCnt, int intPgSz)
        {
            int intVal, intRem;

            intVal = (intMovieCnt / intPgSz);
            intRem = (intMovieCnt % intPgSz);

            if (intRem != 0)
                return(intVal + 1);
            else
                return (intVal);
        }

        private void ParseAndTransfer(DataTable tblMovieTitle, DataTable tblActors, DataTable tblCrew, DataTable tblDesc, int intRow)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SQLConnection].ConnectionString))
            using (var cmd = conn.CreateCommand())
            {
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    ViewData["Message"] = "ERROR: " + e.Message;
                    return;
                }

                cmd.CommandText = "spFillMovieTables";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MovieTitle", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@ActorStr", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@CrewStr", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@CatAndDesc", SqlDbType.VarChar, 255);
                cmd.Parameters["@MovieTitle"].Value = tblMovieTitle.Rows[intRow][1].ToString().Replace("\"","");
                cmd.Parameters["@ActorStr"].Value = tblActors.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.Parameters["@CrewStr"].Value = tblCrew.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.Parameters["@CatAndDesc"].Value = tblDesc.Rows[intRow][1].ToString().Replace("\"", "");
                cmd.CommandTimeout = 30;
                cmd.Prepare();

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    ViewData["Message"] = "ERROR: " + e.Message;
                    return;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

    }
}