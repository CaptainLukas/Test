using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Seq.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Textanalyse.Data.Data;
using Hangfire;
using Hangfire.SQLite;
using System.Diagnostics;
using Textanalyse.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Textanalyse.Web.Controllers;

namespace Textanalyse.Web
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
            services.AddScoped<ITextContext>(provider => provider.GetService<TextContext>());
            services.AddTransient<IRepository, Repository>();
            services.AddDbContext<TextContext>(options => options.UseSqlite("Data Source=sqlite.db", x => x.MigrationsAssembly("Textanalyse.Web")));
            services.AddLocalization();
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix).AddDataAnnotationsLocalization();
            
            //services.AddHangfire(x => x.UseSqlServerStorage("C:\\Users\\acer\\Source\\Repos\\TextAnalyse\\Textanalyse.Web"));

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSeq();
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<TextContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ITextContext context)
        {
            context.Migrate();

            app.UseAuthentication();

            var cultures = new[] { new CultureInfo ("en"), new CultureInfo ("de") };

            var localisationOptions = new RequestLocalizationOptions { DefaultRequestCulture = new RequestCulture("en"), SupportedCultures = cultures, SupportedUICultures = cultures };

            app.UseRequestLocalization(localisationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            /*/ Hangfire configuration
            var options = new SQLiteStorageOptions();
            GlobalConfiguration.Configuration.UseSQLiteStorage("Data Source = E:\\OneDrive\\SourceCodes\\Study\\ASPNetHangfireSQLite\\ASPNetHangfireSQLite\\App_Data\\Hangfire.sqlite", options);
            var option = new BackgroundJobServerOptions { WorkerCount = 1 };
            app.UseHangfireServer(option);
            app.UseHangfireDashboard();
            // Add scheduled jobs
            RecurringJob.AddOrUpdate(() => Run(), Cron.Minutely);*/

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        public void Run()
        {
            Debug.WriteLine($"Run at {DateTime.Now}");
        }
    }
}
