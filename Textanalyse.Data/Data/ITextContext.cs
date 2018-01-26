using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Data
{
    public interface ITextContext
    {
        DbSet<Text> Text { get; set; }

        void Migrate();
    }
}
