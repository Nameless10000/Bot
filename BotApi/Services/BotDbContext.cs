using BotApi.Models.Configurations;
using BotApi.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

            // тут вставлять данные в БД через modelBuilder.Entity<>().HasData();

            base.OnModelCreating(modelBuilder);
        }
    }
}
