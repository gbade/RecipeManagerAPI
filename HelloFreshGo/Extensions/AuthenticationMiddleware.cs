using HelloFreshGo.Business.Contracts;
using HelloFreshGo.Util.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HelloFreshGo.Extensions
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHelloFreshConfigManager _config;
        private readonly IStringHelper _decrypt;

        public AuthenticationMiddleware(
            RequestDelegate next,
            IHelloFreshConfigManager config,
            IStringHelper decrypt)
        {
            _config = config;
            _decrypt = decrypt;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string authUsername = _config.AuthUsername;
                string authPassword = _config.AuthPassword;

                //Extract credentials
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                var actualUser = _decrypt.DecryptCredentials(authUsername);
                var actualPass = _decrypt.DecryptCredentials(authPassword);

                if (username == actualUser && password == actualPass)
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }
            else
            {
                // no authorization header
                context.Response.StatusCode = 401; //Unauthorized
                return;
            }
        }
    }
}
