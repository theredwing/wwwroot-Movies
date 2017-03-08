using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class NamesLU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NamesLUID { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Name: ")]
        public string Name { get; set; }
    }
}