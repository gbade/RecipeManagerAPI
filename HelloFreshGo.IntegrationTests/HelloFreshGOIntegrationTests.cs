using FluentAssertions;
using HelloFreshGo.Business.Contracts;
using HelloFreshGo.Business.Managers;
using HelloFreshGo.Entities.Models;
using HelloFreshGo.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloFreshGo.IntegrationTests
{
    public class HelloFreshGOIntegrationTests : IDisposable
    {
        private TestServer server;

        public HttpClient Client { get; private set; }

        public HelloFreshGOIntegrationTests()
        {
            string curDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

            var builder = new ConfigurationBuilder()
                .SetBasePath(curDir)
                .AddJsonFile("appsettings.json");

            var webBuilder = new WebHostBuilder()
                .UseContentRoot(curDir).UseConfiguration(builder.Build())
                .UseStartup<Startup>();

            server = new TestServer(webBuilder);

            Client = server.CreateClient();
        }

        [Fact]
        public async Task Recipes_Get_All()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.GetAsync("/api/hellofresh/recipes");

                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Recipes_GetById_ReturnsOk()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.GetAsync("/api/hellofresh/recipes/5");
                
                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Recipes_GetById_ReturnsNotFound()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.GetAsync("/api/hellofresh/recipes/1000");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Recipes_GetById_ReturnsBadRequest()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.GetAsync("/api/hellofresh/recipes/2A");

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task SaveRecipe_ReturnsOk()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var model = new Recipes
                {
                    Name = "Ewa Agoin",
                    PrepTime = "45 minutes",
                    Difficulty = 2,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/hellofresh/recipes", request);

                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task SaveRecipes_ReturnsBadRequest()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var model = new Recipes
                {
                    PrepTime = "40 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/hellofresh/recipes", request);
                
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task SaveRecipes_ReturnsUnAuthorized()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var model = new Recipes
                {
                    Name = "Efo Riro",
                    PrepTime = "40 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/hellofresh/recipes", request);

                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task RateRecipe_ShouldReturnOk()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var rating = new Ratings
                {
                    RecipeRating = 5
                };

                var json = JsonConvert.SerializeObject(rating);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/hellofresh/recipes/3/rating", request);

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task RateRecipe_ShouldReturnBadRequest()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var rating = new Ratings
                {
                    RecipeRating = 56
                };

                var json = JsonConvert.SerializeObject(rating);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/hellofresh/recipes/3/rating", request);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task SearchForRecipes_ShouldReturnOk()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.GetAsync("/api/hellofresh/recipes/search?preptime=30 minutes");

                response.EnsureSuccessStatusCode();

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task SearchForRecipes_ShouldReturnNotFound()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.GetAsync("/api/hellofresh/recipes/search?name=Efo Riro&preptime=1 hour");

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task UpdateRecipe_ReturnsOk()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var model = new Recipes
                {
                    Name = "Efo Riro",
                    PrepTime = "30 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("/api/hellofresh/recipes/5", request);

                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task UpdateRecipe_ReturnsNotFound()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var model = new Recipes
                {
                    Name = "Efo Riro",
                    PrepTime = "30 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("/api/hellofresh/recipes/100", request);
                
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task UpdateRecipe_ReturnsBadRequest()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var model = new Recipes
                {
                    Name = "Efo Riro",
                    PrepTime = "30 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("/api/hellofresh/recipes/2a", request);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task UpdateRecipe_InvalidModelStateReturnsBadRequest()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var model = new Recipes
                {
                    PrepTime = "30 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("/api/hellofresh/recipes/2", request);

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task UpdateRecipe_ReturnsUnAuthorized()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var model = new Recipes
                {
                    Name = "Efo Riro",
                    PrepTime = "30 minutes",
                    Difficulty = 1,
                    Vegetarian = 1
                };

                var json = JsonConvert.SerializeObject(model);
                var request = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync("/api/hellofresh/recipes/5", request);

                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task DeleteRecipes_ReturnsOk()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var response = await client.DeleteAsync("/api/hellofresh/recipes/8");

                response.EnsureSuccessStatusCode();

                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }        

        [Fact]
        public async Task DeleteRecipes_ReturnsNotFound()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var response = await client.DeleteAsync("/api/hellofresh/recipes/200");

                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        [Fact]
        public async Task DeleteRecipes_ReturnsBadRequest()
        {
            var username = ClientHelper.GetUsername();
            var password = ClientHelper.GetPassword();

            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Basic {Convert.ToBase64String(byteArray)}");

                var response = await client.DeleteAsync("/api/hellofresh/recipes/2AB7");

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task DeleteRecipes_ReturnsUnAuthorized()
        {
            using (var client = new HelloFreshGOIntegrationTests().Client)
            {
                var response = await client.DeleteAsync("/api/hellofresh/recipes/8");

                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        public void Dispose()
        {
            server?.Dispose();
            Client?.Dispose();
        }
    }
}
