using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProjektZaliczeniowy.Data;
using ProjektZaliczeniowy.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektZaliczeniowy
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
            ////wielojezykowosc - bez tego kodu dzia³a zmiana na inny jêzyk w menu powi¹zanych z partial view
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});
            ////.

            services.AddDbContext<CompanyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("CompanyConnection")));
            //linia 50 - dodaliœmy mechanizm ról
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("CompanyConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();

            //Globalny antiforgery token, stosowany przeciwko cross-site scripting
            //DO WSTAWIENIA: 

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            //Implement a strategy to select the language/culture for each request - nasze
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
            //.

            services.Configure<RequestLocalizationOptions>(options => {
                var supportedCultures = new List<CultureInfo> {
                    new CultureInfo("en-US"),
                    new CultureInfo("pl"),
                    new CultureInfo("de"),
                  };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                // Formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //ustawienie domyœlnej kultury na angielski-amerykañski
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseCookiePolicy(); //cookies

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //przekierowanie na kontroler Error, {0} - placeholder np. 404
            app.UseStatusCodePagesWithRedirects("/Error/{0}");

            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            */
            //nasze routy
            app.UseEndpoints(endpoints =>
            {
                //Shortcut routes
                /*
                endpoints.MapControllerRoute(
                    name: "manageEmployee",
                    pattern: "e/{id}/{action}",
                    defaults: new { controller = "Employees", action = "{action}" },
                    constraints: new { action = "^((?!.*create.*).)*$" }
                    );
                */

                endpoints.MapControllerRoute(
                    name: "manageEmployee",
                    pattern: "e/{action}/{id?}",
                    defaults: new { controller = "Employees", action = "{action}" },
                    constraints: new { action = "^((?!.*create.*).)*$" }
                    );

                endpoints.MapControllerRoute(
                    name: "createEmployee",
                    pattern: "e/create",
                    defaults: new { controller = "Employees", action = "Create" }
                    );

                //Redirections
                //TODO: Consider change, maybe instead of 'Index', redirect to the corresponding view, but with first item from the list.
                /*
                endpoints.MapControllerRoute(
                    name: "manageEmployeeRedirect",
                    pattern: "e/{id?}/{action}",
                    defaults: new { controller = "Employees", action = "Index" },
                    constraints: new { action = "^((?!.*create.*).)*$" }
                    );
                */
                /*
                endpoints.MapControllerRoute(
                    name: "manageEmployeeRedirect",
                    pattern: "e/{action}",
                    defaults: new { controller = "Employees", action = "{action}" },
                    constraints: new { action = "^((?!.*create.*).)*$" }
                    );
                */
                /*
                //zmieniæ lub wywaliæ
                //Unique routes 
                //TODO: Change it to details for last 10 employees.
                endpoints.MapControllerRoute(
                    name: "editTop10Employees",
                    pattern: "top10/{id}/edit",
                    defaults: new { controller = "Employees", action = "Edit" },
                    constraints: new { id = "[1-9]|10" }
                    );
                */

                //DO WSTAWIENIA:
                //Overwrite Home route
                endpoints.MapControllerRoute(
                    name: "home",
                    pattern: "/Home",
                    defaults: new { controller = "Employees", action = "Index" }
                    );

                //Default route
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Employees}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
    }
}
