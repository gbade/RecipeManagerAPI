using HelloFreshGo.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloFreshGo.Business.Contracts
{
    public interface IRecipeManager
    {
        Task<Recipes> Save(Recipes model);

        Task<Ratings> RateRecipe(long id, Ratings ratings);

        Task<IEnumerable<Recipes>> SearchForRecipe(string name, string preptime);

        //Task<IEnumerable<Recipes>> SearchForAllRecipes(UrlQuery urlQuery);

        Recipes Details(long? id);

        Task<List<Recipes>> GetRecipes();

        Task<Recipes> DeleteRecipe(long? id);

        Task<Recipes> EditRecipe(long? id, Recipes model);       
    }
}
