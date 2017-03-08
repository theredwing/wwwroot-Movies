using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class PositionsLU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PositionsLUID { get; set; }
        public string Position { get; set; }
    }
}