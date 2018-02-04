using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Textanalyse.Web.Entities
{
    public class Word
    {
        public Word()
        {
            Value = "";
        }

        public Word(string word)
        {
            this.Value = word;
        }

        public  int WordID
        {
            get;
            set;
        }

        public int SentenceID
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }
}
