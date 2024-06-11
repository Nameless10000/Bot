using BotApi.Models.Configurations;
using BotApi.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace BotApi.Services
{
    public class BotDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<WorkerDiscipline> WorkerDisciplines { get; set; }

        public BotDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worker>().HasData(
                new Worker { ID = 806499592, UserName = "znya05" }
                );

            modelBuilder.Entity<User>().HasData(
                new User { ID = 659615698, UserName = "Quazzik" }
                );

            modelBuilder.Entity<Discipline>().HasData(
                new Discipline { ID=1, Name="Заработок на росте криптовалют в порно-играх"}
                );

            modelBuilder.Entity<WorkerDiscipline>().HasData(
                new WorkerDiscipline { DisciplineID = 1, WorkerID = 806499592 }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
