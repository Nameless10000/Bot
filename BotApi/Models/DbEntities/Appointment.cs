using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotApi.Models.DbEntities
{
    public class Appointment
    {
        [Key]
        public int ID { get; set; }

        public long UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        public long WorkerID { get; set; }

        [ForeignKey(nameof(WorkerID))]
        public Worker Worker { get; set; }

        public int DisciplineID { get; set; }

        [ForeignKey(nameof(DisciplineID))]
        public Discipline Discipline { get; set; }

        public TimeSpan Longevity { get; set; }

        public DateTime StartsAt { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public bool IsCompleted => StartsAt <= DateTime.Now;

        public bool IsNotified { get; set; }
    }
}
