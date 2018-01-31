using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Data
{
    public interface ITextContext
    {
        DbSet<Text> Text { get; set; }
        DbSet<Sentence> Sentence { get; set; }
        DbSet<Word> Word { get; set; }

        void Migrate();
    }
}
