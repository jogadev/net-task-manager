using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Services;
using TaskManager.Models;
using BCrypt.Net;
using System.IO;
using System.Text;

namespace TaskManager.Controllers.Middleware
{
    public class UserValidations
    {
        private readonly RequestDelegate _next;
        
        public UserValidations(RequestDelegate next)
        {
            _next = next;
        }

        public async System.Threading.Tasks.Task Invoke(HttpContext context, UserService srv)
        {
            string method = context.Request.Method,
                   path = context.Request.Path;
            
            if(method == "POST" && path == "/users")
            {
                Console.WriteLine("#########BODY");
                string bodyReq = "";
                context.Request.EnableBuffering();
                using(StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyReq = await reader.ReadToEndAsync();
                }
                context.Request.Body.Position = 0;
                Console.WriteLine(bodyReq);
            }

            await _next(context);
        }


        private void hashPassword(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }

        private bool validatePassword(string pass)
        {
            if (pass.Contains("password") || pass.Length < 6)
                return false;
            else
                return true;
        }


    }
}
