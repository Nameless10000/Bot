namespace BotAPILib
{
    public static class CoreRequests
    {
        private static readonly HttpClient _httpClient = new();
        public static async Task<string> GetBotTokenAsync(string pass)
        {
            var res = await _httpClient.GetAsync($"http://localhost:5274/api/BotHostAPI/GetToken?pwd={pass}");
            var token = await res.Content.ReadAsStringAsync();
            return token;
        }
    }
}