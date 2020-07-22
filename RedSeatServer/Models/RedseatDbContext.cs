using System;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace RedSeatServer.Models
{
    public class RedseatDbContext : DbContext
    {
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Downloader> Downloaders { get; set; }
        public static string sqlitePath() {
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
                .Entity<Download>()
                .Property(e => e.DownloadStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Download>()
                .HasIndex(b => b.DownloadStatus);
            modelBuilder
                .Entity<Downloader>()
                .Property(e => e.DownloaderType)
                .HasConversion<string>();
        }
    }

    public enum DownloadStatus {
        None,
        Pending,
        Downloading,
        Downloaded,
        Uploading,
        Done
    }

}