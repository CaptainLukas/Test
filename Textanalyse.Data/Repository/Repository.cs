using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Data.Data;
using System.Collections;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Repository
{
    public class Repository : IRepository
    {
        private TextContext context;

        public Repository(TextContext context)
        {
            this.context = context;
        }
        public void SaveText(string text)
        {

        }

        public void AnalyseText(Text text, string[] Suchbegriffe)
        {

        }


        private SentenceResult Satzcheck(Sentence sentence, string[] Suchbegriffe)
        {
            SentenceResult result = new SentenceResult();

            result.SentenceID = sentence.SentenceID;

            

            return result;
        }

        private int NebensatzCheck(List<Word> words, string[] Suchbegriffe)
        {
            int score = 0;

            return score;/////////////////////////////////////
        }

        private int Hauptsatzcheck(List<Word> words, string[] Suchbegriffe)
        {
            int score = 0;

            for (int i = 0; i < Suchbegriffe.Length; i++)
            {

            }

            return score;////////////////////////////////////////////
        }

        private int LevenshteinDistance(string wordOne, string wordTwo)
        {
            if (string.IsNullOrEmpty(wordOne))
            {
                if (!string.IsNullOrEmpty(wordTwo))
                {
                    return wordTwo.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(wordTwo))
            {
                if (!string.IsNullOrEmpty(wordOne))
                {
                    return wordOne.Length;
                }
                return 0;
            }

            int cost;
            int[,] d = new int[wordOne.Length + 1, wordTwo.Length + 1];
            int min1;
            int min2;
            int min3;

            for (int i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (int i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (int i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (int j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    cost = Convert.ToInt32(!(wordOne[i - 1] == wordTwo[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

        }


        private class SentenceResult
        {
            public int SentenceID { get; set; }

            public int score { get; set; }

            public string  summary { get; set; }
        }
    }
}
