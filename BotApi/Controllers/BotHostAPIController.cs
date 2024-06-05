using BotApi.Models.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BotApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BotHostAPIController : ControllerBase
    {
        private BotData _botData;

        public BotHostAPIController(IOptions<BotData> botData)
        {
            _botData = botData.Value;
        }

        [HttpGet]
        public async Task<JsonResult> GetToken([FromQuery] string pwd)
        {
            if (pwd != _botData.Pwd)
                return new("Вы долбаеб");

            return new(_botData.Token);
        }
    }
}
