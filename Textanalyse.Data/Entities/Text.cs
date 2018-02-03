using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Textanalyse.Web.Entities
{
    public class Text
    {
        public int TextID
        {
            get;
            set;
        }

        public string Owner
        {
            get;
            set;
        }

        public string OriginalText
        {
            get;
            set;
        }

        public List<Sentence> Sentences
        {
            get;
            set;
        }
    }
}
