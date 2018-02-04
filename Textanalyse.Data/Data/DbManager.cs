using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Data
{
    public class DbManager
    {
        public TextContext context;

        public DbManager(TextContext textContext)
        {
            context = textContext;
        }

        public void AddText(string newText, string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                owner = "unknown";
            }

            if (string.IsNullOrWhiteSpace(newText))
            {
                return;
            }

            Text text = new Text();
            text.OriginalText = newText;
            text.Owner = owner;

            string[] newSentences = newText.Split(new char[] { '.', '!', '?'}, StringSplitOptions.RemoveEmptyEntries);

            foreach(string sentence in newSentences)
            {
                text.Sentences.Add(new Sentence());
            }

            try
            {
                context.Add(text);
                context.SaveChanges();
            }
            catch(Exception e)
            {
                //log exception
            }

            foreach(Sentence sentence in text.Sentences)
            {
                sentence.TextID = text.TextID;
            }

            for (int i = 0; i < newSentences.Length; i++)
            {
                if (text.Sentences[i].SentenceID <= 1)
                {
                    text.Sentences[i].BeforeSentenceID = -1;
                }
                else
                {
                    text.Sentences[i].BeforeSentenceID = text.Sentences[i].SentenceID - 1;
                }

                if (newSentences.Length - 1 == i)
                {
                    text.Sentences[i].NextSentenceID = -1;
                }
                else
                {
                    text.Sentences[i].NextSentenceID = text.Sentences[i].SentenceID + 1;
                }

                string[] newWords = newSentences[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

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

            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                //log exception
            }
        }
    }
}
