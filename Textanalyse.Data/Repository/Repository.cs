using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Data.Data;
using System.Collections;
using Textanalyse.Web.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Textanalyse.Data.Repository
{
    public class Repository : IRepository
    {
        private TextContext context;

        private DbManager manager;

        private readonly ILogger log;

        public Repository(TextContext context, ILogger<Repository> log)
        {
            this.context = context;
            this.manager = new DbManager(context);
            this.log = log;
        }
        public void SaveText(string text, string owner)
        {
            try
            {
                this.manager.AddText(text, owner);
                log.LogInformation("New text added.", text);
            }
            catch (Exception e)
            {
                log.LogError("Saving the new text failed.", e);
            }
        }

        public void RemoveTextByID(int id)
        {
            try
            {
                context.Text.Remove(context.Text.Where(x => x.TextID == id).ToList()[0]);
                context.SaveChanges();
                log.LogInformation("Text removed.");
            }
            catch(Exception e)
            {
                log.LogError("Removing text failed.", e);
            }
        }

        public void EditText(Text text, string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
            {
                return;
            }

            Text new_Text = new Text();
            new_Text.OriginalText = newText;
            new_Text.Owner = text.Owner;

            string[] sentences = newText.Split(new string[] { ".", "!", "?" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string sentence in sentences)
            {
                new_Text.Sentences.Add(new Sentence());
            }
            
            try
            {
                context.Add(new_Text);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                log.LogError("Editing the text failed.", e);
                return;
            }

            for (int i = 0; i < sentences.Length; i++)
            {
                if (text.Sentences[i].SentenceID <= 1)
                {
                    text.Sentences[i].BeforeSentenceID = -1;
                }
                else
                {
                    text.Sentences[i].BeforeSentenceID = text.Sentences[i].SentenceID - 1;
                }

                if (sentences.Length - 1 == i)
                {
                    text.Sentences[i].NextSentenceID = -1;
                }
                else
                {
                    text.Sentences[i].NextSentenceID = text.Sentences[i].SentenceID + 1;
                }

                string[] newWords = sentences[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < newWords.Length; j++)
                {
                    string newWord = newWords[j].Replace(",", string.Empty);
                    newWord = newWords[j].Replace("(", string.Empty);
                    newWord = newWords[j].Replace(")", string.Empty);
                    newWord = newWords[j].Replace(";", string.Empty);
                    newWord = newWords[j].Replace("-", string.Empty);
                    Word word = new Word(newWord);
                    word.SentenceID = text.Sentences[i].SentenceID;
                    text.Sentences[i].Words.Add(word);
                }
            }

            text.Sentences = new_Text.Sentences;
            text.OriginalText = new_Text.OriginalText;

            for (int i = 0; i < new_Text.Sentences.Count; i++)
            {
                text.Sentences[i].TextID = text.TextID;
            }
            new_Text.Sentences = null;
            new_Text.OriginalText = null;
            
            try
            {
                context.Remove(new_Text);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                log.LogError("Editing the text failed.", e);
                return;
            }
        }

        public List<Text> GetTextByOwner(string owner)
        {
            try
            {
                return context.Text.Where(x => x.Owner == owner).Include(y => y.Sentences).ThenInclude(z => z.Words).ToList();
            }
            catch(Exception e)
            {
                //log
                return null;
            }
        }

        public Text GetTextByID(int id)
        {
            try
            {
                return context.Text.Where(x => x.TextID == id).Include(y => y.Sentences).ThenInclude(z => z.Words).ToList()[0];
            }
            catch (Exception e)
            {
                //log
                return null;
            }
        }

        public List<Text> GetTexts()
        {
            try
            {
                List<Text> text = context.Text
                    .Include(x => x.Sentences)
                    .ThenInclude(y => y.Words)
                    .ToList();
                return text;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<TextResult> SearchResult(string suche)
        {
            string[] suchbegriffe = suche.Split(' ');

            if (suchbegriffe.Length > 3)
            { return null; }

            List<TextResult> result = new List<TextResult>();

            List<Text> textList = this.context.Text.Include(x => x.Sentences).ThenInclude(y => y.Words).ToList();

            for (int i = 0; i < textList.Count; i++)
            {
                result.Add(AnalyseText(textList[i], suchbegriffe, textList));
            }

            while(result.Count > 5)
            {
                int min = 0;

                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].Score < result[min].Score)
                    {
                        min = i;
                    }
                }

                result.RemoveAt(min);
            }

            return result;
        }

        private TextResult AnalyseText(Text text, string[] suchbegriffe, List<Text> texts)
        {
            List<SentenceResult> results = new List<SentenceResult>();

            TextResult textResult = new TextResult();
            textResult.TextID = text.TextID;

            for (int i = 0; i < text.Sentences.Count; i ++)
            {
                results.Add(Satzcheck(text.Sentences[i], suchbegriffe, texts, text.TextID));

                textResult.Score += results[i].Score;
            }

            for (int j = 0; j < results.Count; j++)
            {
                if (textResult.Sentences.Count < 3)
                {
                    textResult.Sentences.Add(results[j]);
                }
                else
                {
                    int min = 0;

                    for (int k = 0; k < textResult.Sentences.Count; k++)
                    {
                        if (textResult.Sentences[k].Score < textResult.Sentences[min].Score)
                        {
                            min = k;
                        }
                    }

                    if (textResult.Sentences[min].Score < results[j].Score)
                    {
                            textResult.Sentences.RemoveAt(min);
                            textResult.Sentences.Add(results[j]);
                    }
                }
            }

            return textResult;
        }


        private SentenceResult Satzcheck(Sentence sentence, string[] suchbegriffe, List<Text> textList, int textID)
        {
            SentenceResult result = new SentenceResult();

            result.SentenceID = sentence.SentenceID;

            Sentence vorsatz, nachsatz;

            if (sentence.BeforeSentenceID != -1)
            {
                 vorsatz = textList[textID].Sentences[sentence.BeforeSentenceID];
            }
            else
            {
                vorsatz = new Sentence();
            }
            
            if (sentence.NextSentenceID != -1)
            {
                nachsatz = textList[textID].Sentences[sentence.NextSentenceID];
            }
            else
            {
                nachsatz = new Sentence();
            }

            for (int i = 0; i < suchbegriffe.Length; i++)
            {
                SearchTerm searchterm = new SearchTerm(suchbegriffe[i]);
                //Vorsatz Score
                HauptSatzCheck(vorsatz.Words, searchterm, SatzTyp.Previoussentence);

                //Nachsatz Score
                HauptSatzCheck(nachsatz.Words, searchterm, SatzTyp.Nextsentence);

                //Hauptsatz Score
                HauptSatzCheck(sentence.Words, searchterm, SatzTyp.Mainsentence);

                result.Score = searchterm.MainSentence * 3 + searchterm.NextSentence * 2 + searchterm.PreviousSentence * 2;

                result.Summary.Add(result.Score.ToString() + " Points: Term " + searchterm.OriginalTerm + " found " + searchterm.MainSentence.ToString() + "x in main sentence, " + searchterm.PreviousSentence.ToString() + "x in previous sentence and " + searchterm.NextSentence.ToString() + "x in next sentence");

                for (int j = 0; j < searchterm.SimilarTerms.Count; j++)
                {
                    int tempscore = 0;

                    tempscore = searchterm.SimilarTerms[j].MainSentence * 2 + searchterm.SimilarTerms[j].NextSentence * 1 + searchterm.SimilarTerms[j].PreviousSentence * 1;

                    result.Summary.Add(tempscore.ToString() + " Points: Similar Term for " + searchterm.OriginalTerm + " [" + searchterm.SimilarTerms[j].Term + "] found " + searchterm.SimilarTerms[j].MainSentence.ToString() + "x in main sentence, " + searchterm.SimilarTerms[j].PreviousSentence.ToString() + "x in previous sentence and " + searchterm.SimilarTerms[j].NextSentence.ToString() + "x in next sentence");

                    result.Score += tempscore;
                }
            }

            return result;
        }
        
        private void HauptSatzCheck(List<Word> words, SearchTerm searchterm, SatzTyp typ)
        {
            for (int i = 0; i < words.Count; i++)
            {
                //wenn selbes Wort im Satz
                if (searchterm.OriginalTerm == words[i].Value)
                {
                    switch(typ)
                    {
                        case SatzTyp.Mainsentence:
                            searchterm.MainSentence++;
                            break;
                        case SatzTyp.Nextsentence:
                            searchterm.NextSentence++;
                            break;
                        case SatzTyp.Previoussentence:
                            searchterm.PreviousSentence++;
                            break;
                    }
                }
                else
                {
                    //Wenn ähnliches Wort im Satz ist
                    if (LevenshteinDistance(searchterm.OriginalTerm, words[i].Value) == 1)
                    {
                        bool IsFound = false;

                        // checken obs schon mal vorgekommen ist
                        for (int j = 0; j < searchterm.SimilarTerms.Count; j++)
                        {
                            if (searchterm.SimilarTerms[j].Term == words[i].Value)
                            {
                                switch (typ)
                                {
                                    case SatzTyp.Mainsentence:
                                        searchterm.SimilarTerms[j].MainSentence++;
                                        break;
                                    case SatzTyp.Nextsentence:
                                        searchterm.SimilarTerms[j].NextSentence++;
                                        break;
                                    case SatzTyp.Previoussentence:
                                        searchterm.SimilarTerms[j].PreviousSentence++;
                                        break;
                                }

                                IsFound = true;
                            }
                        }

                        // wenn nicht vorgekommen, zu vorgekommen liste
                        if (!IsFound)
                        {
                            SimilarTerm newTerm = new SimilarTerm(words[i].Value);

                            searchterm.SimilarTerms.Add(newTerm);

                            switch (typ)
                            {
                                case SatzTyp.Mainsentence:
                                    newTerm.MainSentence++;
                                    break;
                                case SatzTyp.Nextsentence:
                                    newTerm.NextSentence++;
                                    break;
                                case SatzTyp.Previoussentence:
                                    newTerm.PreviousSentence++;
                                    break;
                            }
                        }
                    }
                }
            }
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
        
        private class SearchTerm
        {
            public SearchTerm(string searchterm)
            {
                this.OriginalTerm = searchterm;
                this.MainSentence = 0;
                this.PreviousSentence = 0;
                this.NextSentence = 0;
                this.SimilarTerms = new List<SimilarTerm>();
            }

            public string OriginalTerm { get; private set; }

            public int MainSentence { get; set; }

            public int PreviousSentence { get; set; }

            public int NextSentence { get; set; }

            public List<SimilarTerm> SimilarTerms {get; set;}
        }

        private class SimilarTerm
        {
            public SimilarTerm( string term )
            {
                this.MainSentence = 0;
                this.NextSentence = 0;
                this.PreviousSentence = 0;
            }

            public string Term { get; private set; }

            public int MainSentence { get; set; }

            public int PreviousSentence { get; set; }

            public int NextSentence { get; set; }
        }

        public enum SatzTyp
        {
            Mainsentence,
            Previoussentence,
            Nextsentence,
        }
    }
}
