using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Movie: ")]
        public string MovieTitle { get; set; }
        [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters")]
        [Display(Name = "Description: ")]
        public string Description { get; set; }
        public virtual ICollection<MovieCategory> MovieCategory { get; set; }
        public virtual ICollection<MovieNamesPosition> MovieNamesPosition { get; set; }
        public IEnumerable<CategoryLU> Category { get; private set; }
        public IEnumerable<NamesLU> Names { get; private set; }
        public IEnumerable<PositionsLU> PositionsLU { get; private set; }

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }
    }

    public class MovieCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieCategoryId { get; set; }
        public int MovieId { get; set; }
        public int CategoryLUID { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual CategoryLU Category { get; set; }
    }

    public class MovieNamesPosition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MovieNamesPositionID { get; set; }
        public int MovieId { get; set; }
        public int NamesId { get; set; }
        public int PositionId { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual NamesLU NamesLU { get; set; }
        public virtual PositionsLU PositionsLU { get; set; }
    }

}
