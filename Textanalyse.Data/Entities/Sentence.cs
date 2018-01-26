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

        public Sentence BeforeSentence
        {
            get;
            set;
        }

        public Sentence NextSentence
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
