using System;
using System.Collections.Generic;
using System.Text;

namespace Textanalyse.Data.Repository
{
    public class SentenceResult
    {
            public SentenceResult()
            {
                this.Summary = new List<string>();
                this.Score = 0;
            }

            public int SentenceID { get; set; }

            public int Score { get; set; }

            public List<string> Summary { get; set; }
    }
}
