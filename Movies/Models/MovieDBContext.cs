using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Movies.Models
{
    public class MovieDBContext : DbContext
    {
        public MovieDBContext() : base("SQLConnection")
        {
            Database.SetInitializer<MovieDBContext>(new CreateDatabaseIfNotExists<MovieDBContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        //public DbSet<MovieV> MovieV { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieCategory> MovieCategorys { get; set; }
        public DbSet<MovieNamesPosition> MovieNamesPositions { get; set; }
        public DbSet<PositionsLU> PositionsLU { get; set; }
        public DbSet<NamesLU> NamesLU { get; set; }
        public DbSet<CategoryLU> Categorys { get; set; }
    }
}