﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RedSeatServer.Models;

namespace RedSeatServer.Migrations
{
    [DbContext(typeof(RedseatDbContext))]
    [Migration("20200814210641_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-preview.6.20312.4");

            modelBuilder.Entity("RedSeatServer.Models.Download", b =>
                {
                    b.Property<int>("DownloadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DownloadStatus")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("Downloaded")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DownloaderId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExternalId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("FilesAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("DownloadId");

                    b.HasIndex("DownloadStatus");

                    b.HasIndex("DownloaderId");

                    b.ToTable("Downloads");
                });

            modelBuilder.Entity("RedSeatServer.Models.Downloader", b =>
                {
                    b.Property<int>("DownloaderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DownloaderType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.HasKey("DownloaderId");

                    b.ToTable("Downloaders");
                });

            modelBuilder.Entity("RedSeatServer.Models.Episode", b =>
                {
                    b.Property<int?>("ShowId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Season")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AbsoluteNumber")
                        .HasColumnType("INTEGER");

                    b.Property<long>("FirstAired")
                        .HasColumnType("INTEGER");

                    b.Property<long>("LastUpdated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Overview")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("ShowId", "Season", "Number");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("RedSeatServer.Models.Genre", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Name", "ShowId");

                    b.HasIndex("ShowId");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("RedSeatServer.Models.RFile", b =>
                {
                    b.Property<int>("fileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DownloadId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EpisodeNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EpisodeSeason")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EpisodeShowId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Parsed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ShowId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.HasKey("fileId");

                    b.HasIndex("DownloadId");

                    b.HasIndex("ShowId");

                    b.HasIndex("EpisodeShowId", "EpisodeSeason", "EpisodeNumber");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("RedSeatServer.Models.Show", b =>
                {
                    b.Property<int>("ShowId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("Added")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AirsDayOfWeek")
                        .HasColumnType("TEXT");

                    b.Property<long>("AirsTime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Aliases")
                        .HasColumnType("TEXT");

                    b.Property<long?>("FirstAired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImdbId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<long>("LastUpdated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Overview")
                        .HasColumnType("TEXT");

                    b.Property<string>("Rating")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Runtime")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Season")
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TvdbId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Zap2itId")
                        .HasColumnType("TEXT");

                    b.HasKey("ShowId");

                    b.HasIndex("TvdbId")
                        .IsUnique();

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("RedSeatServer.Models.Token", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.HasKey("Name");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("RedSeatServer.Models.Download", b =>
                {
                    b.HasOne("RedSeatServer.Models.Downloader", "Downloader")
                        .WithMany("Downloads")
                        .HasForeignKey("DownloaderId");
                });

            modelBuilder.Entity("RedSeatServer.Models.Episode", b =>
                {
                    b.HasOne("RedSeatServer.Models.Show", null)
                        .WithMany("Episodes")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RedSeatServer.Models.Genre", b =>
                {
                    b.HasOne("RedSeatServer.Models.Show", null)
                        .WithMany("Genre")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RedSeatServer.Models.RFile", b =>
                {
                    b.HasOne("RedSeatServer.Models.Download", "Download")
                        .WithMany("Files")
                        .HasForeignKey("DownloadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RedSeatServer.Models.Show", "Show")
                        .WithMany()
                        .HasForeignKey("ShowId");

                    b.HasOne("RedSeatServer.Models.Episode", "Episode")
                        .WithMany()
                        .HasForeignKey("EpisodeShowId", "EpisodeSeason", "EpisodeNumber");
                });
#pragma warning restore 612, 618
        }
    }
}
