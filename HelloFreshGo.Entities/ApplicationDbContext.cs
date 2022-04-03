using System;
using HelloFreshGo.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloFreshGo.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Recipes> Recipes { get; set; }
        public DbSet<Ratings> Ratings { get; set; }


    }
}
