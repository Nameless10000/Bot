using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotApi.Models.DbEntities
{
    [PrimaryKey(nameof(WorkerID), nameof(DisciplineID))]
    public class WorkerDiscipline
    {
        public long WorkerID { get; set; }

        [ForeignKey(nameof(WorkerID))]
        public Worker Worker { get; set; }

        public int DisciplineID { get; set; }

        [ForeignKey(nameof(DisciplineID))]
        public Discipline Discipline { get; set; }
    }
}
