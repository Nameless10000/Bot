using System.ComponentModel.DataAnnotations;

namespace BotApi.Models.DbEntities
{
    public class Worker
    {
        [Key]
        public long ID { get; set; }

        public string UserName { get; set; }
    }
}
