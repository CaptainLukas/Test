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
            


        }

        public void EditText()
        {

        }

        public void DeleteText()
        {

        }
    }
}
