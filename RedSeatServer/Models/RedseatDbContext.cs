using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RedSeatServer.Models
{
    public class RedseatDbContext : DbContext
    {
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Downloader> Downloaders { get; set; }
        public DbSet<RFile> Files { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public static string sqlitePath()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"redseat");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return Path.Combine(folder, "redseat.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={sqlitePath()}");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
               .Entity<Genre>()
               .HasKey(nameof(Genre.Name));

                modelBuilder
               .Entity<Token>()
               .HasKey(nameof(Token.Name));

            modelBuilder
               .Entity<Episode>()
               .HasKey(nameof(Episode.Season), nameof(Episode.Number));
            modelBuilder
               .Entity<Episode>()
                .Property(e => e.FirstAired)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
               .Entity<Episode>()
                .Property(e => e.LastUpdated)
                .HasConversion(new DateTimeOffsetToBinaryConverter());

            modelBuilder
               .Entity<Show>()
               .HasKey(nameof(Show.ShowId));
            modelBuilder
               .Entity<Show>()        
                .HasIndex(s => s.TvdbId)
                .IsUnique();;
            modelBuilder
               .Entity<Show>()
                .Property(e => e.Status)
                .HasConversion<string>();
            modelBuilder
               .Entity<Show>()
                .Property(e => e.FirstAired)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
               .Entity<Show>()
                .Property(e => e.LastUpdated)
                .HasConversion(new DateTimeOffsetToBinaryConverter());  

            modelBuilder
               .Entity<Show>()
                .Property(e => e.AirsDayOfWeek)
                .HasConversion<string>();
            modelBuilder
               .Entity<Show>()
                .Property(e => e.AirsTime)
                .HasConversion(new TimeSpanToTicksConverter());
            modelBuilder
               .Entity<Show>()
                .Property(e => e.Added)
                .HasConversion(new DateTimeOffsetToBinaryConverter());
            modelBuilder
                .Entity<Show>()
                .Property(e => e.Aliases)
                .HasConversion(
                    v => string.Join('|', v),
                    v => v.Split('|', StringSplitOptions.RemoveEmptyEntries));

            modelBuilder
               .Entity<Download>()
               .HasMany(c => c.Files)
               .WithOne(e => e.Download)
               .IsRequired();
            modelBuilder
                .Entity<Download>()
                .Property(e => e.DownloadStatus)
                .HasConversion<string>();
            modelBuilder
                .Entity<Download>()
                .Property(e => e.Type)
                .HasConversion<string>();
            modelBuilder.Entity<Download>()
                .HasIndex(b => b.DownloadStatus);
            modelBuilder
                .Entity<Downloader>()
                .Property(e => e.DownloaderType)
                .HasConversion<string>();
        }
    }

    public enum DownloadStatus
    {
        None,
        Pending,
        Downloading,
        Downloaded,
        Uploading,
        Done
    }

}