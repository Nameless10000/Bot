using System.Net;

namespace BotApi.Models.DTOs
{
    public class BotResponseDTO
    {
        public HttpStatusCode Code { get; set; }
        public string? Message { get; set; }
    }
}