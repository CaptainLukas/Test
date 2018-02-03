using System;
using System.Collections.Generic;
using System.Text;

namespace Textanalyse.Data.Repository
{
    public class TextResult
    {
        public TextResult()
        {
            this.Sentences = new List<SentenceResult>();
            this.Score = 0;
        }

        public int TextID { get; set; }

        public int Score { get; set; }

        public List<SentenceResult> Sentences { get; set; }
    }
}
