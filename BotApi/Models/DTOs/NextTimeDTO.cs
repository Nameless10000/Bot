using BotApi.Models.DbEntities;

namespace BotApi.Models.DTOs
{
    public class NextTimeDTO
    {
        public DateTime NextAvailableTime { get; set; }
        public Worker Worker { get; set; }
    }
}
