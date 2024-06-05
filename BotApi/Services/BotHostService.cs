using BotApi.Models.DbEntities;
using BotApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BotApi.Services
{
    public class BotHostService
    {
        private readonly BotDbContext _botDbContext;

        public BotHostService(BotDbContext botDbContext)
        {
            _botDbContext = botDbContext;
        }

        public async Task<JsonResult> GetWorkersAsync()
        {
            return new(await _botDbContext.Workers.ToListAsync());
        }

        public async Task<JsonResult> GetDisciplinesAsync()
        {
            return new(await _botDbContext.Disciplines.ToListAsync());
        }

        public async Task<JsonResult> GetWorkersByDisciplineAsync([FromQuery] int disciplineID)
        {
            return new(await _botDbContext.WorkerDisciplines
                .Include(x => x.Worker)
                .Where(x => x.DisciplineID == disciplineID)
                .Select(x => x.Worker)
                .ToListAsync());
        }

        public async Task<List<NextTimeDTO>> GetNextAvailableTimeAsync([FromBody] NextAvailableRequestParams requestParams)
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
                .Select(x => new NextTimeDTO
                {
                    Worker = x.Worker,
                    NextAvailableTime = x.StartsAt - now < TimeSpan.FromMinutes(30)
                        ? x.StartsAt + x.Longevity + TimeSpan.FromMinutes(15)
                        : now
                })
                .ToList());
        }

        public async Task<bool> AppointToWorkerAsync(AppointmentCreatingDTO creatingDTO)
        {
            var isWorkerAvailable = !(await _botDbContext.Appointments
                .Where(x => x.WorkerID == creatingDTO.WorkerID)
                .ToListAsync())
                .Any(x => (creatingDTO.StartsAt < x.StartsAt + x.Longevity 
                && creatingDTO.StartsAt > x.StartsAt)
                || (creatingDTO.StartsAt + creatingDTO.Longevity < x.StartsAt + x.Longevity 
                && creatingDTO.StartsAt + creatingDTO.Longevity > x.StartsAt));

            if (!isWorkerAvailable)
                return false;

            var appointment = new Appointment
            {
                WorkerID = creatingDTO.WorkerID,
                UserID = creatingDTO.UserID,
                DisciplineID = creatingDTO.DisciplineID,
                StartsAt = creatingDTO.StartsAt,
                Longevity = creatingDTO.Longevity
            };

            await _botDbContext.Appointments.AddAsync(appointment);
            await _botDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddUserAsync(UserAddDTO addDTO)
        {
            if (!await _botDbContext.Users.AnyAsync(x => x.ID == addDTO.ID))
                return false;

            var newUser = new User
            {
                ID = addDTO.ID,
                UserName = addDTO.UserName
            };

            await _botDbContext.Users.AddAsync(newUser);
            await _botDbContext.SaveChangesAsync();
            return true;
        } 
    }
}
