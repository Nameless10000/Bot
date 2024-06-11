﻿// <auto-generated />
using System;
using BotApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BotApi.Migrations
{
    [DbContext(typeof(BotDbContext))]
    partial class BotDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("BotApi.Models.DbEntities.Appointment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int>("DisciplineID")
                        .HasColumnType("int");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<TimeSpan>("Longevity")
                        .HasColumnType("time(6)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("StartsAt")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserID")
                        .HasColumnType("bigint");

                    b.Property<long>("WorkerID")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("DisciplineID");

                    b.HasIndex("UserID");

                    b.HasIndex("WorkerID");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("BotApi.Models.DbEntities.Discipline", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("Disciplines");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Name = "Заработок на росте криптовалют в порно-играх"
                        });
                });

            modelBuilder.Entity("BotApi.Models.DbEntities.User", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            ID = 659615698L,
                            UserName = "Quazzik"
                        });
                });

            modelBuilder.Entity("BotApi.Models.DbEntities.Worker", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ID");

                    b.ToTable("Workers");

                    b.HasData(
                        new
                        {
                            ID = 806499592L,
                            UserName = "znya05"
                        });
                });

            modelBuilder.Entity("BotApi.Models.DbEntities.WorkerDiscipline", b =>
                {
                    b.Property<long>("WorkerID")
                        .HasColumnType("bigint");

                    b.Property<int>("DisciplineID")
                        .HasColumnType("int");

                    b.HasKey("WorkerID", "DisciplineID");

                    b.HasIndex("DisciplineID");

                    b.ToTable("WorkerDisciplines");

                    b.HasData(
                        new
                        {
                            WorkerID = 806499592L,
                            DisciplineID = 1
                        });
                });

            modelBuilder.Entity("BotApi.Models.DbEntities.Appointment", b =>
                {
                    b.HasOne("BotApi.Models.DbEntities.Discipline", "Discipline")
                        .WithMany()
                        .HasForeignKey("DisciplineID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotApi.Models.DbEntities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotApi.Models.DbEntities.Worker", "Worker")
                        .WithMany()
                        .HasForeignKey("WorkerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Discipline");

                    b.Navigation("User");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("BotApi.Models.DbEntities.WorkerDiscipline", b =>
                {
                    b.HasOne("BotApi.Models.DbEntities.Discipline", "Discipline")
                        .WithMany()
                        .HasForeignKey("DisciplineID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BotApi.Models.DbEntities.Worker", "Worker")
                        .WithMany()
                        .HasForeignKey("WorkerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Discipline");

                    b.Navigation("Worker");
                });
#pragma warning restore 612, 618
        }
    }
}
