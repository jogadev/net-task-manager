using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Services;
using TaskManager.Models;

namespace TaskManager.Controllers.Middleware
{
    public class Authentication
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, string[]> allowedRoutes;

        public Authentication(RequestDelegate  next)
        {
            _next = next;
            allowedRoutes = new Dictionary<string, string[]>
            {
                { "PATCH", new [] { "/users/me" } }
            };
        }

        public async System.Threading.Tasks.Task Invoke(HttpContext context, UserService srv)
        {
            if (!RequiresAuthentication(context.Request.Method, context.Request.Path))
                await _next(context);
            else
                try
                {
                    string authHeader = context.Request.Headers["Authorization"];
                    if (authHeader == null)
                        sendUnauthenticatedMessage(context);
                    else
                    {
                        string token = authHeader.Replace("Bearer ", "");
                        string targetId = TokenManager.Verify(token);
                        User authenticatedUser = srv.VerifyUserWithToken(targetId, token);
                        if (authenticatedUser == null)
                            sendUnauthenticatedMessage(context);
                        else
                        {
                            context.Items["_id"] = authenticatedUser.Id;
                            await _next(context);
                        }
                    }
                }
                catch (ArgumentNullException)
                {
                    sendUnauthenticatedMessage(context);
                }
        }
        
        public bool RequiresAuthentication(string method, string path)
        {
            try
            {
                return allowedRoutes[method].Contains<string>(path);
            }catch(Exception) // I am assuming that accessing a non-existing key would end up here.
            {
                return false;
            }
        }

        private async static void sendUnauthenticatedMessage(HttpContext context)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Please authenticate");
        }
    }
}
