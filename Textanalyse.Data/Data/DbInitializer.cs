using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Data
{
    public static class DbInitializer
    {
        public static void Seed(TextContext context)
        {
            //using (var context = new TextContext(
            //    applicationBuilder.GetRequiredService<DbContextOptions<TextContext>>()))
            //{
            // Look for any movies.

            if (context.Text.Any())
            {
                return;   // DB has been seeded
            }

            Word[] wordsCool = { new Word("Hallo"), new Word("ich"), new Word("bin"), new Word("ein"), new Word("cooler"), new Word("Typ") };
            Word[] wordsTest = { new Word("Das"), new Word("ist"), new Word("en"), new Word("coole"), new Word("Test"), new Word("Satz") };
            Word[] wordsProgrammieren = { new Word("Programmieren"), new Word("ist"), new Word("mein"), new Word("liebstes"), new Word("Fach"), new Word("ever") };
            Word[] wordsTesto = { new Word("Ich"), new Word("mach"), new Word("hier"), new Word("einen"), new Word("coolen"), new Word("Testo") };
            Word[] wordsHUE = { new Word("Ih"), new Word("mache"), new Word("so"), new Word("liebsten"), new Word("meine"), new Word("Fache") };

            Sentence sentenceCool = new Sentence();
            Sentence sentenceTest = new Sentence();
            Sentence sentenceProgrammieren = new Sentence();
            Sentence sentenceTesto = new Sentence();
            Sentence sentenceHUE = new Sentence();

            for (int i = 0; i < wordsCool.Length; i++)
            {
                sentenceCool.Words.Add(wordsCool[i]);
                sentenceTest.Words.Add(wordsTest[i]);
                sentenceProgrammieren.Words.Add(wordsProgrammieren[i]);
                sentenceTesto.Words.Add(wordsTesto[i]);
                sentenceHUE.Words.Add(wordsHUE[i]);
            }

            context.Text.AddRange(
                new Text
                {
                    Sentences = new List<Sentence>() { sentenceCool, sentenceProgrammieren , sentenceTesto, sentenceCool },
                },

                new Text
                {
                    Sentences = new List<Sentence>() { sentenceCool, sentenceTest , sentenceHUE, sentenceHUE },
                },

                 new Text
                 {
                     Sentences = new List<Sentence>() { sentenceProgrammieren , sentenceTest, sentenceTesto , sentenceTest },
                 },

                 new Text
                 {
                     Sentences = new List<Sentence>() { sentenceHUE, sentenceTesto, sentenceProgrammieren, sentenceTesto },
                 }
            );
            context.SaveChanges();
            //}
        }

        private static Dictionary<string, Text> _texts;
        public static Dictionary<string, Text> Texts
        {
            get
            {
                if (_texts == null)
                {
                    // Add categories...
                }

                return _texts;
            }
        }
    }
}
