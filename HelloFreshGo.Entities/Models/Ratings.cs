using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HelloFreshGo.Entities.Models
{
    public class Ratings
    {
        public long Id { get; set; }

        [Required]
        public int RecipeRating { get; set; }

        [Required]
        public long RecipeId { get; set; }
    }
}
