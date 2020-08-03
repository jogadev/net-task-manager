using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Mongo;
using TaskManager.Services;
using TaskManager.Controllers.Middleware;
using Microsoft.AspNetCore.Http;

namespace TaskManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var secretKey = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"));

            services.Configure<TaskManagerDatabaseSettings>(Configuration.GetSection(nameof (TaskManagerDatabaseSettings)));
            services.AddSingleton<ITaskManagerDatabaseSettings>(sp => sp.GetRequiredService<IOptions<TaskManagerDatabaseSettings>>().Value);
            services.AddSingleton<UserService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseWhen(matchEndpoint("GET", "/users"), MiddlewareCollection.RequireToken);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private Func<HttpContext, bool> matchEndpoint(string method, string path)
        {
            return ctx =>
            {
                Console.Write($"{ctx.Request.Method}  {ctx.Request.Path}   ::   {ctx.Request.Method == method && ctx.Request.Path == path}");
                return ctx.Request.Method == method && ctx.Request.Path == path;
            };
        }
    }
}
