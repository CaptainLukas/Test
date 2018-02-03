using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Textanalyse.Data.Data;

namespace Textanalyse.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())//Azure täuschungs Kommentar
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<TextContext>();
                    DbInitializer initializer = new DbInitializer(context);
                    initializer.Seed(context);
                }
                catch(Exception e)
                {
                    var logger = services.GetRequiredService <ILogger< Program >> ();
                    logger.LogError(e, "An error occurred while seeding the database");
                }
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
