using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcUser.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using MvcUser.Models;

namespace MvcUser
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
            services.AddControllersWithViews();

            //services.AddDbContext<MvcUserContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("MvcUserContext")));

            //services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));

            //services.AddDbContext<MvcUserContext>(options =>
            //options.UseMySql(Configuration.GetConnectionString("MvcUserContext")));

            //services.Add(new ServiceDescriptor(typeof(MvcUserContext), new MvcUserContext(Configuration.GetConnectionString("DefaultConnection"))));
            services.AddTransient<AppDb>(_ => new AppDb(Configuration["ConnectionStrings:DefaultConnection"]));

            services.AddDistributedMemoryCache();


            services.AddMvc();

            services.AddSession(option =>       //Enable to use Session
            {
                option.IdleTimeout = TimeSpan.FromSeconds(10);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();  //Enable to use Session

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "Shared",
                   pattern: "Shared/{*_Layout}",
                   defaults: new { controller = "Shared", action = "_Layout" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
