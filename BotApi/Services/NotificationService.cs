﻿using BotApi.Models.Configurations;
using BotApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;

namespace BotApi.Services
{
    public class NotificationService(BotDbContext _dbContext, IOptions<BotData> _botData)
    {
        private HttpClient _httpClient = new();

        public async void SendDailyAppointments()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var appointments = await _dbContext
                .Appointments
                .Include(x => x.User)
                .Include(x => x.Discipline)
                .Where(x => x.StartsAt > today && x.StartsAt < tomorrow)
                .GroupBy(x => x.User)
                .ToListAsync();

            var models = appointments
                //.GroupBy(x => x.User)
                .Select(x => new NotificationDTO
                {
                    ChatID = x.Key.ID,
                    UserName = x.Key.UserName,
                    Message = x.Aggregate($"{x.Key.UserName}, ваши записи на сегодня:\n", (acc, cur) => acc += $"{cur.Discipline.Name} в ${cur.StartsAt:t}")
                })
                .ToList();

            await _httpClient.PostAsJsonAsync(_botData.Value.Path, models);
        }

        public async void SendNearestAppointments()
        {
            var now = DateTime.Now;

            var appointments = await _dbContext
                .Appointments
                .Include(x => x.User)
                .Include(x => x.Discipline)
                .Where(x => x.StartsAt > now && x.StartsAt < now.AddHours(1) && !x.IsNotified)
                .GroupBy(x => x.User)
                .ToListAsync();

            var models = appointments
                .Select(x => new NotificationDTO
                {
                    UserName = x.Key.UserName,
                    ChatID = x.Key.ID,
                    Message = x.Aggregate($"{x.Key.UserName}, ваши записи на сегодня:\n", (acc, cur) => acc += $"{cur.Discipline.Name} в ${cur.StartsAt:t}")
                });

            await _httpClient.PostAsJsonAsync(_botData.Value.Path, models);
        }
    }
}
