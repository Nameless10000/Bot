using System.ComponentModel.DataAnnotations;

namespace BotApi.Models.DbEntities
{
    public class User
    {
        [Key]
        public long ID { get; set; }

        public string? UserName { get; set; }
    }
}
