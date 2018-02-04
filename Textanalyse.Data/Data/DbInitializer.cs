using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Data
{
    public class DbInitializer
    {
        private TextContext context;

        private DbManager manager;

        private readonly ILogger log;

        public DbInitializer(TextContext textContext)
        {
            context = textContext;
            manager = new DbManager(textContext);
            this.log = new Logger<DbManager>(new LoggerFactory());
        }

        public void Seed(TextContext context)
        {
            if (context.Text.Any())
            {
                return;
            }

            manager.AddText("Hallo ich hab einen Test Satz. Das ist der Text zum testen meines Programmes. Bitte suche nach Test. Tes. Ich hab viel spaß.", "Lukas");

            manager.AddText("Ich habe diesen Text selbst geschrieben. Es ist ein Test für die Suche, ein Tet. Ich brauch noch ein paar Sätze. Vielleicht sollte ic Schriftsteller werden.","Lukas");

            manager.AddText("Die Windmühle ist ein technisches Bauwerk, das mittels seiner vom Wind in Drehung versetzten Flügel Arbeit verrichtet. Am verbreitetsten war die Nutzung als Mühle, wodurch die Bezeichnung auf alle derartigen Anlagen übertragen wurde. Windmühlen waren, neben den an Standorten mit nutzbarer Wasserkraft anzutreffenden Wassermühlen, bis zur Erfindung der Motoren die einzigen frühen Kraftmaschinen nach der Muskelkraftmaschine in der Menschheitsgeschichte.Entsprechend vielfältig war ihre Verwendung als Mahlmühle, als Ölmühle, zur Verarbeitung von Werkstoffen.","Lukas");

            try
            {
                context.SaveChanges();
                log.LogInformation("Database is seeded.");
            }
            catch (Exception e)
            {
                log.LogError("Error while Adding Text.", e.Message);
            }
        }
    }
}
