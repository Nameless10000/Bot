using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotHost;

public class Program
{
    private static List<string> _commandsAvailable = new()
    {
        "/someMessage"
    };

    static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

<<<<<<< Updated upstream
=======
    private static async Task MainAsync()
    {

    }

>>>>>>> Stashed changes

}
