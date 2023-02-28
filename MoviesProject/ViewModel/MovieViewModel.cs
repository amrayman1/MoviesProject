using MoviesProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MoviesProject.VeiwModel
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        public int GenreId { get; set; }
        [Display(Name = "Genre")]
        public IEnumerable<Genre> Genres { get; set; }
        public string Year { get; set; }
    }
}