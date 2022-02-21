using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DotNetCore5CRUD.Models;

namespace DotNetCore5CRUD.ViewModels
{
    public class AnimesFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        [Range(1,10)]
        public double Rate { get; set; }

        [Required]
        [StringLength(2500)]
        public string Story { get; set; }

        [Display(Name ="Select poster...")]
        public byte[] Poster { get; set; }

        [Display(Name ="Category")]
        public byte CategoryId { get; set; }
        public List<Category> Categories { get; internal set; }
    }
}
