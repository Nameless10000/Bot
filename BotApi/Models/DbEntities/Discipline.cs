using System.ComponentModel.DataAnnotations;

namespace BotApi.Models.DbEntities
{
    public class Discipline
    {
        [Key]
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
