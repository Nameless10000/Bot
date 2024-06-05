using Microsoft.EntityFrameworkCore;

namespace BotApi.Models.DbEntities
{
    [PrimaryKey(nameof(WorkerID), nameof(DisciplineID))]
    public class WorkerDiscipline
    {
        public long WorkerID { get; set; }

        public int DisciplineID { get; set; }
    }
}
