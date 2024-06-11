using BotApi.Models.Configurations;
using BotApi.Models.DbEntities;
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
        private readonly BotData _botData;
        private readonly BotHostService _botHostService;

        public BotHostAPIController(IOptions<BotData> botData, BotHostService botHostService)
        {
            _botData = botData.Value;
            _botHostService = botHostService;
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
            return new(await _botHostService.GetWorkersAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetDisciplines()
        {
            return new(await _botHostService.GetDisciplinesAsync());
        }

        [HttpGet]
        public async Task<JsonResult> GetWorkersByDiscipline([FromQuery] int disciplineID)
        {
            return new(await _botHostService.GetWorkersByDisciplineAsync(disciplineID));
        }

        [HttpPost]
        public async Task<JsonResult> GetNextAvailableTime([FromBody] NextAvailableRequestParams requestParams)
        {
            return new(await _botHostService.GetNextAvailableTimeAsync(requestParams));
        }

        [HttpPost]
        public async Task<IActionResult> AppointToWorker(AppointmentCreatingDTO creatingDTO)
        {
            return await _botHostService.AppointToWorkerAsync(creatingDTO) ? Ok() : BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserAddDTO addDTO)
        {
            return await _botHostService.AddUserAsync(addDTO) ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<JsonResult> GetAppointments([FromQuery] long userID)
        {
            return new(await _botHostService.GetAppointmentsAsync(userID));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAppointment([FromQuery] int appointmentID)
        {
            return await _botHostService.DeleteAppointmentAsync(appointmentID) ? Ok() : BadRequest();
        }
    }
}
