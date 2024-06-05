namespace BotApi.Models.DTOs
{
    public class AppointmentCreatingDTO
    {
        public TimeSpan Longevity { get; set; }

        public DateTime StartsAt { get; set; }

        public long WorkerID { get; set; }

        public long UserID {  get; set; }

        public int DisciplineID { get; set; }
    }
}
