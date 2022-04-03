using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HelloFreshGo.Entities.Models
{
    public class Recipes
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PrepTime { get; set; }
        [Required]
        public int Difficulty { get; set; }
        [Required]
        public Int16 Vegetarian { get; set; }
    }
}
