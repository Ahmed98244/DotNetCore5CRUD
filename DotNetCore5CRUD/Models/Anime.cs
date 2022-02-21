using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5CRUD.Models
{
    public class Anime
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; }

        public int Year { get; set; }

        public double Rate { get; set; }

        [Required]
        [MaxLength(2500)]
        public string Story { get; set; }

        [Required]
        public byte[] Poster { get; set; }

        public byte CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
