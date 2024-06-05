using BotApi.Models.Configurations;
using BotApi.Models.DTOs;
using BotApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BotApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BotHostAPIController : ControllerBase
    {
        private readonly BotDbContext _botDbContext;
        private readonly BotData _botData;

        public BotHostAPIController(IOptions<BotData> botData, BotDbContext botDbContext)
        {
            _botData = botData.Value;
            _botDbContext = botDbContext;
        }

        [HttpGet]
        public async Task<JsonResult> GetToken([FromQuery] string pwd)
        {
            if (pwd != _botData.Pwd)
                return new("Вы долбаеб");

            return new(_botData.Token);
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkers()
        {
            return new( await _botDbContext.Workers.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetDisciplines()
        {
            return new(await _botDbContext.Disciplines.ToListAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkersByDiscipline([FromQuery] int disciplineID)
        {
            return new(await _botDbContext.WorkerDisciplines
                .Include(x => x.Worker)
                .Where(x => x.DisciplineID == disciplineID)
                .Select(x => x.Worker)
                .ToListAsync());
        }

        public async Task<JsonResult> GetNextAvailableTime([FromQuery] NextAvailableRequestParams requestParams)
        {
            var nextTimeQuery = _botDbContext.Appointments
                .Include(x => x.Worker)
                .Where(x => x.DisciplineID == requestParams.DisciplineID)
                .AsQueryable();

            if (requestParams.WorkerID != null)
                nextTimeQuery = nextTimeQuery
                    .Where(x => x.WorkerID == requestParams.WorkerID)
                    .AsQueryable();

            var now = DateTime.Now;

            var nextTime = await nextTimeQuery.ToListAsync();

            return new(nextTime
                .Select(x => new { x.Worker, NextAvailableTime = x.StartsAt - now > TimeSpan.FromMinutes(30) 
                    ? x.StartsAt + x.Longevity + TimeSpan.FromMinutes(15) 
                    : now })
                .ToList());
        }
    }
}
