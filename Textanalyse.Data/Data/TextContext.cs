using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Textanalyse.Web.Entities;

namespace Textanalyse.Data.Data
{
    public class TextContext : IdentityDbContext<ApplicationUser>, ITextContext
    {
        public DbSet<Text> Text { get; set; }
        public DbSet<Sentence> Sentence { get; set; }
        public DbSet<Word> Word { get; set; }

        public TextContext(DbContextOptions<TextContext> options) : base(options) { }

        public void Migrate()
        {
            this.Database.Migrate();
        }
    }
}
