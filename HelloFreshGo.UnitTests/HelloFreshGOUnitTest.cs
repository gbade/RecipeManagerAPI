using HelloFreshGo.Business.Contracts;
using HelloFreshGo.Business.Managers;
using HelloFreshGo.Controllers;
using HelloFreshGo.Entities;
using HelloFreshGo.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using GenFu;
using System.Linq;
using System.Threading.Tasks;

namespace HelloFreshGo.UnitTests
{   
    public class HelloFreshGOUnitTest 
    {
        private readonly Mock<IRecipeManager> _recipeMock = new Mock<IRecipeManager>();
        private readonly Mock<ILogger<HelloFreshController>> mockLogger = new Mock<ILogger<HelloFreshController>>();

        private IEnumerable<Recipes> GetFakeData()
        {
            var i = 1;
            var recipes = A.ListOf<Recipes>(26);
            recipes.ForEach(x => x.Id = i++);
            return recipes.Select(_ => _);
        }

        [Fact]
        public void ShouldSaveRecipes()
        {
            //Arrange
            ILogger<HelloFreshController> _logger = mockLogger.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object,_logger);

            Recipes model = new Recipes
            {
                Name = "Chili Mac",
                PrepTime = "1 hour",
                Difficulty = 2,
                Vegetarian = 1
            };

            //Act
            var actionResult = _helloFreshController.Create(model);
            var okResult = actionResult as OkObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void UpdateRecipeById_ReturnOk()
        {
            //Arrange
            ILogger<HelloFreshController> _logger = mockLogger.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);
            var recipe = new Mock<Recipes>().Object;

            //Act
            var result = _helloFreshController.UpdateRecipe(1, recipe);
            var response = result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateRecipeById_IdNotFound()
        {
            //Arrange
            ILogger<HelloFreshController> _logger = mockLogger.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);
            var recipe = new Mock<Recipes>().Object;

            //Act
            var result = _helloFreshController.UpdateRecipe(100, recipe);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ShouldRateRecipes_ReturnOk()
        {
            //Arrange
            ILogger<HelloFreshController> _logger = mockLogger.Object;

            var recipes = GetFakeData();
            var recipe = recipes.First();
            _recipeMock.Setup(x => x.Details(1)).Returns(recipe);

            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);

            Ratings model = new Ratings { RecipeRating = 3 };

            //Act
            var actionResult = _helloFreshController.RateRecipe(1, model);
            var okResult = actionResult as OkObjectResult;

            //Assert
            Assert.NotNull(actionResult);
            Assert.IsType<ObjectResult>(actionResult);
        }

        [Fact]
        public void ListAllRecipes_ReturnsOk()
        {
            //Arrange 
            var mock = new Mock<ILogger<HelloFreshController>>();
            ILogger<HelloFreshController> _logger = mock.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);

            //Act
            var result = _helloFreshController.ListRecipes();
            var okResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetRecipeById_ReturnsOk()
        {
            //Arrange 
            var mock = new Mock<ILogger<HelloFreshController>>();
            ILogger<HelloFreshController> _logger = mock.Object;
            
            var recipes = GetFakeData();
            var recipe = recipes.First();
            _recipeMock.Setup(x => x.Details(1)).Returns(recipe);

            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);
            
            //Act
            var result = _helloFreshController.GetRecipe(1);
            var res = result as OkObjectResult; 

            //Assert
            Assert.NotNull(res);
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void GetRecipeById_NotFound()
        {
            //Arrange 
            ILogger<HelloFreshController> _logger = mockLogger.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);

            //Act
            var result = _helloFreshController.GetRecipe(90);
            var response = result as NotFoundResult;

            //Assert
            Assert.Equal(404, response.StatusCode);
            Assert.IsNotType(result.GetType(), typeof(OkResult));
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public void DeleteRecipeById_ReturnsOk()
        {
            //Arrange 
            ILogger<HelloFreshController> _logger = mockLogger.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);

            //Act
            var result = _helloFreshController.DeleteRecipe(2);
            var okResult = result as OkObjectResult;

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void DeleteRecipeById_ReturnIdNotFound()
        {
            //Arrange 
            ILogger<HelloFreshController> _logger = mockLogger.Object;
            var _helloFreshController = new HelloFreshController(_recipeMock.Object, _logger);

            //Act
            var result = _helloFreshController.DeleteRecipe(1000);

            //Assert
            Assert.IsNotType(result.GetType(), typeof(NotFoundResult));
        }
        
    }
}
