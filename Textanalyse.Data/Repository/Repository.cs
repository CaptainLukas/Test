using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Data.Data;

namespace Textanalyse.Data.Repository
{
    class Repository
    {
        private TextContext context;

        public Repository(TextContext context)
        {
            this.context = context;
        }
    }
}
