using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Data.Data;
using System.Collections;

namespace Textanalyse.Data.Repository
{
    public class Repository : IRepository
    {
        private TextContext context;

        public Repository(TextContext context)
        {
            this.context = context;
        }
        public void SaveText(string text)
        {

        }
    }
}
