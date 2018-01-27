using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Textanalyse.Web.Entities
{
    public class Sentence
    {
        public int SentenceID
        {
            get;
            set;
        }

        public int BeforeSentenceID
        {
            get;
            set;
        }

        public int NextSentenceID
        {
            get;
            set;
        }

        public List<Word> Words
        {
            get;
            set;
        }
    }
}
