using System.Collections.Generic;
using System.Linq;
using HelloFreshGo.Business.Contracts;
using HelloFreshGo.Entities;
using HelloFreshGo.Entities.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dapper;
using static Dapper.SqlMapper;
using MySql.Data.MySqlClient;

namespace HelloFreshGo.Business.Managers
{
    public class RecipeManager : IRecipeManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IHelloFreshConfigManager _configManager;

        public RecipeManager(ApplicationDbContext context, IHelloFreshConfigManager configManager)
        {
            _context = context;
            _configManager = configManager;
        }

        public async Task<Recipes> Save(Recipes model)
        {
            _context.Recipes.Add(model);
            model.Id = await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Ratings> SaveRatings(Ratings model)
        {
            _context.Ratings.Add(model);
            model.Id = await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Ratings> RateRecipe(long id, Ratings ratings)
        {
            var model = new Ratings();
            model.RecipeId = id;
            model.RecipeRating = ratings.RecipeRating;
            

            model = await SaveRatings(model);

            return model;
        }

        public Recipes Details(long? id)
        {          
            var item = _context.Recipes.Where(s => s.Id == id).FirstOrDefault();
            return item;
        }

        public async Task<Recipes> DeleteRecipe(long? id)
        {            
            var item = Details(id);
            if (item != null)
            {
                _context.Recipes.Remove(item);
                await _context.SaveChangesAsync();
            }

            return item;
        }

        public async Task<Recipes> EditRecipe(long? id, Recipes model)
        {
            var item = Details(id);
            if (item != null)
            {
                item.Name = model.Name;
                item.PrepTime = model.PrepTime;
                item.Difficulty = model.Difficulty;
                item.Vegetarian = model.Vegetarian;
                
                _context.Recipes.Update(item);
                await _context.SaveChangesAsync();
            }
            
            return item;
        }

        public async Task<List<Recipes>> GetRecipes()
        {
            var recipes = await _context.Recipes.ToListAsync();

            return recipes;
        }

        public async Task<IEnumerable<Recipes>> SearchForRecipe(string name, string preptime)
        {
            IEnumerable<Recipes> recipes = null;
            var urlQuery = new UrlQuery();
            urlQuery.Recipe.Name = name;
            urlQuery.Recipe.PrepTime = preptime;

            var settings = _configManager.HelloFreshConnection;

            using (var connection = new MySqlConnection(settings))
            {
                connection.Open();

                string sql = @"SELECT Name, PrepTime, Difficulty, Vegetarian FROM Recipes";
                if (urlQuery.HaveFilter)
                {
                    string filterSQL = "";
                    if (!string.IsNullOrEmpty(urlQuery.Recipe.Name))
                    {
                        filterSQL += " Name = @Name";
                    }
                    if (!string.IsNullOrEmpty(urlQuery.Recipe.PrepTime))
                    {
                        if (!string.IsNullOrEmpty(filterSQL))
                        {
                            filterSQL += " AND";
                        }
                        filterSQL += " PrepTime LIKE @PrepTime";
                    }
                    sql += $" WHERE {filterSQL}";
                }
                
                recipes = await connection.QueryAsync<Recipes>(sql, new
                {
                    Name = urlQuery.Recipe.Name,
                    PrepTime = $@"%{urlQuery.Recipe.PrepTime}%"
                });
            }

            return recipes;
        }
    }
}
