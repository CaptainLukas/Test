using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Repository
{
    public interface IRepository
    {
        void SaveText(string text, string owner);

        void EditText(Text text, string newText);

        List<Text> GetTextByOwner(string owner);

        Text GetTextByID(int id);

        void RemoveTextByID(int id);

        List<TextResult> SearchResult(string suche);
    }
}
