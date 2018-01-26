﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        public TextContext(DbContextOptions<TextContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Sentence>()
                .HasMany(w => w.Words);
        }

        public void Migrate()
        {
            this.Database.Migrate();
        }
    }
}
