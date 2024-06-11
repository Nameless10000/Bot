using BotApi.Models.DbEntities;
using BotApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace BotApi.Services
{
    public class BotHostService
    {
        private readonly BotDbContext _botDbContext;

        public BotHostService(BotDbContext botDbContext)
        {
            _botDbContext = botDbContext;
        }

        public async Task<List<Worker>> GetWorkersAsync()
        {
            return new(await _botDbContext.Workers.ToListAsync());
        }

        public async Task<List<Discipline>> GetDisciplinesAsync()
        {
            return new(await _botDbContext.Disciplines.ToListAsync());
        }

        public async Task<List<Worker>> GetWorkersByDisciplineAsync(int disciplineID)
        {
            return new(await _botDbContext.WorkerDisciplines
                .Include(x => x.Worker)
                .Where(x => x.DisciplineID == disciplineID)
                .Select(x => x.Worker)
                .ToListAsync());
        }

        public async Task<List<NextTimeDTO>> GetNextAvailableTimeAsync(NextAvailableRequestParams requestParams)
        {
            var workers = (await GetWorkersByDisciplineAsync(requestParams.DisciplineID))
                .Select(x => new NextTimeDTO
                {
                    Worker = x,
                    NextAvailableTime = DateTime.Now,
                }).ToList();

            var nextTimeQuery = _botDbContext.Appointments
                .Include(x => x.Worker)
                .Where(x => x.DisciplineID == requestParams.DisciplineID)
                .AsQueryable();

            if (requestParams.WorkerID != null)
                nextTimeQuery = nextTimeQuery
                    .Where(x => x.WorkerID == requestParams.WorkerID)
                    .AsQueryable();

            var now = DateTime.Now;

            var nextTime = (await nextTimeQuery.ToListAsync())
                .Select(x => new NextTimeDTO
                {
                    Worker = x.Worker,
                    NextAvailableTime = x.StartsAt - now < TimeSpan.FromMinutes(75)
                            ? x.StartsAt + x.Longevity + TimeSpan.FromMinutes(15)
                            : now
                }).ToList();

            return nextTime.Concat(workers).DistinctBy(x => x.Worker.ID).ToList();
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
            if (await _botDbContext.Users.AnyAsync(x => x.ID == addDTO.ID))
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

        public async Task<List<Appointment>> GetAppointmentsAsync(long userID)
        {
            var appointments = await _botDbContext.Appointments
                .Include(x => x.Discipline)
                .Include(x => x.Worker)
                .Where(x => x.UserID == userID)
                .ToListAsync();
            return appointments;
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentID)
        {
            var appointment = await _botDbContext.Appointments.FirstOrDefaultAsync(x => x.ID == appointmentID);
            if (appointment != null)
            {
                _botDbContext.Appointments.Remove(appointment);
                await _botDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
