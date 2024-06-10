using BotApi.Models.DbEntities;
using BotApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace BotAPILib
{
    public static class CoreRequests
    {
        private static readonly HttpClient _httpClient = new();

        public static async Task<string> GetBotTokenAsync(string pass)
        {
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");
            var res = await _httpClient.GetAsync($"http://localhost:5274/api/BotHostAPI/GetToken?pwd={pass}");
            var token = await res.Content.ReadAsStringAsync();
            return token.Replace("\"", "");
        }

        public static async Task<List<Worker>> GetWorkers()
        {
            var res = await _httpClient.GetAsync("http://localhost:5274/api/BotHostAPI/GetWorkers");
            var result = await res.Content.ReadFromJsonAsync<List<Worker>>();
            return result;
        }

        public static async Task<List<Discipline>> GetDisciplines()
        {
            var res = await _httpClient.GetAsync("http://localhost:5274/api/BotHostAPI/GetDisciplines");
            var result = await res.Content.ReadFromJsonAsync<List<Discipline>>();
            return result;
        }

        public static async Task<List<Worker>> GetWorkersByDiscipline(int ID)
        {
            var res = await _httpClient.GetAsync($"http://localhost:5274/api/BotHostAPI/GetWorkersByDiscipline?disciplineID={ID}");
            var result = await res.Content.ReadFromJsonAsync<List<Worker>>();
            return result;
        }

        public static async Task<List<NextTimeDTO>> GetNextAvailableTime(int disciplineID)
        {
            var cont = JsonContent.Create(new NextAvailableRequestParams { DisciplineID=disciplineID});
            var response = await _httpClient.PostAsync($"http://localhost:5274/api/BotHostAPI/GetNextAvailableTime", cont);
            var nextTimes = await response.Content.ReadFromJsonAsync<List<NextTimeDTO>>();
            return nextTimes;
        }

        public static async Task<bool> AddUser(long id, string username)
        {
            var user = new UserAddDTO() { ID = id, UserName=username};
            var cont = JsonContent.Create(user);
            var response = await _httpClient.PostAsync("http://localhost:5274/api/BotHostAPI/AddUser", cont);
            return response.IsSuccessStatusCode;
        }

        public static async Task<bool> AppointToWorkerAsync(AppointmentCreatingDTO dto)
        {
            var cont = JsonContent.Create(dto);
            var response = await _httpClient.PostAsync("http://localhost:5274/api/BotHostAPI/AppointToWorker", cont);
            return response.IsSuccessStatusCode;
        }

        public static async Task<List<Appointment>> GetAppointments(long userID)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5274/api/BotHostAPI/GetAppointments?userID={userID}");
            var appointments = await response.Content.ReadFromJsonAsync<List<Appointment>>();
            return appointments;
        }
    }
}