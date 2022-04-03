using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HelloFreshGo.IntegrationTests.Helpers
{
    public static class ClientHelper
    {
        public static HttpClient GetClient(string username, string password)
        {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));

            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
                //Set some other client defaults like timeout / BaseAddress
            };
            return client;
        }

        // Auth with bearer token
        public static HttpClient GetClient(string token)
        {
            var authValue = new AuthenticationHeaderValue("Bearer", token);

            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
                //Set some other client defaults like timeout / BaseAddress
            };
            return client;
        }

        public static IConfigurationRoot BuildConfig()
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json").Build();

            return config;
        }

        public static string GetUsername()
        {

            var config = BuildConfig();

            var username = config.GetSection("HelloFreshConfigManager").
                                    GetSection("AuthUsername").Value;

            return username;
        }

        public static string GetPassword()
        {
            var config = BuildConfig();

            var password = config.GetSection("HelloFreshConfigManager").
                                    GetSection("AuthPassword").Value;

            return password;
        }
    }

    public static class HttpClientExtentions
    {
        public static AuthenticationHeaderValue ToAuthHeaderValue(this string username, string password)
        {
            return new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.Encoding.ASCII.GetBytes(
                            $"{username}:{password}")));
        }
    }
}
