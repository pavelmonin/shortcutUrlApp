
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using ShortcutUrlApp.Domain;

namespace ShortcutUrlApp.Data
{
    public class ShortcutUrlContext : DbContext
    {
        public ShortcutUrlContext(DbContextOptions<ShortcutUrlContext> options) : base(options)
        { }

        public DbSet<Url> Urls { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Url>(entity => {
                entity.HasIndex(u => u.Original).IsUnique();
            });

            modelBuilder.Entity<Url>(entity => {
                entity.HasIndex(u => u.Shortened).IsUnique();
            });

            modelBuilder.Entity<Url>()
                .Property(b => b.Original)
                .IsRequired();

            modelBuilder.Entity<Url>()
                .Property(b => b.Shortened)
                .IsRequired()
                .HasMaxLength(6)
                ;

            modelBuilder.Entity<Url>()
                .Property(b => b.ConversionCount)
                .HasDefaultValue(0);

            //shadow properties
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.Name).Property<DateTime>("Created");
                modelBuilder.Entity(entityType.Name).Property<DateTime>("LastModified");
            }

        }


    }
}
