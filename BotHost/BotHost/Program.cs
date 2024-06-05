using BotAPILib;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Net;
using System.Reflection.Metadata;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotHost;

public class Program
{
    private static TelegramBotClient _botClient;
    private static readonly InputFile errorSticker = InputFile.FromString("CAACAgIAAxkBAAEF7KFmYH1DPjnUE7L5aqRhpc14nyANPwACEDwAAt-BkUpgkS6UfM10vTUE");

    private static List<string> _commandsAvailable = new()
    {
        "/start"
    };

    static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        var token = await CoreRequests.GetBotTokenAsync("qwerty");
        _botClient = new TelegramBotClient(token);
        _botClient.StartReceiving(UpdateHandler, ErrorHandler);
        Console.ReadLine();
    }

    private static async Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await ProcedureMessageAsync(update);
    }

    private static async Task ProcedureMessageAsync(Update update)
    {
        if (update.Message != null)
        {
            await ProcedureCommand(update.Message);
        }
    }

    private static async Task ProcedureCommand (Message message, MessageEntity entity = null)
    {
        if (entity == null || entity.Type != MessageEntityType.BotCommand)
            await _botClient.SendStickerAsync(
                chatId: message.Chat.Id,
                sticker: errorSticker
                );
            await _botClient.SendTextMessageAsync(message.Chat, "Введите команду");

    }
}
