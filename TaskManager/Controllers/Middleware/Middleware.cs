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
    public class MiddlewareCollection
    {
        public static void RequireToken(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                //bool isAuthenticated;
                try
                {
                    string authHeader = context.Request.Headers["Authorization"];
                    if (authHeader == null) 
                        sendUnauthenticatedMessage(context);
                    else
                    {
                        string token = authHeader.Replace("Bearer ", "");
                        string targetId = TokenManager.Verify(token);
                        User authenticatedUser = UserService.VerifyUserWithToken(targetId, token);
                        if (authenticatedUser == null)
                            sendUnauthenticatedMessage(context);
                        else
                        {
                            context.Items["_id"] = authenticatedUser.Id;
                            await next();
                        }
                    }
                }
                catch (ArgumentNullException)
                {
                    sendUnauthenticatedMessage(context);
                }
            });
        }


        private async static void sendUnauthenticatedMessage(HttpContext context)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Please authenticate");
        }
    }
}
