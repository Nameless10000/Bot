namespace BotAPILib
{
    public class CoreRequests
    {
        private readonly HttpClient _httpClient = new();
        public async Task<string> GetBotTokenAsync(string pass)
        {
            var res = await _httpClient.GetAsync($"http://localhost:6900/api/BotHostAPI/GetToken?pwd={pass}");
            var token = await res.Content.ReadAsStringAsync();
            return token;
        }
    }
}