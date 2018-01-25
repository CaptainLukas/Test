using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Textanalyse.Web.Entities
{
    public class Text
    {
        private int TextID;

        private List<Sentence> sentences;

        private Sentence beforeSentence;

        private Sentence nextSentence;
    }
}
