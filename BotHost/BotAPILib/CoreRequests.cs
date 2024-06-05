using BotApi.Models.DbEntities;
using System.Net.Http.Json;
using System.Text.Json;

namespace BotAPILib
{
    public static class CoreRequests
    {
        private static readonly HttpClient _httpClient = new();

        public static async Task<string> GetBotTokenAsync(string pass)
        {
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
    }
}