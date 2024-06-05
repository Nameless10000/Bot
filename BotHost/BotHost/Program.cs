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
    private static readonly HttpClient _httpClient = new();
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
        var token = await CoreRequests.GetBotTokenAsync("v4)xh_VN>%539~r:J6sYD7");
        _botClient = new TelegramBotClient(token, _httpClient);
        _botClient.StartReceiving(UpdateHandler, ErrorHandler);
        Console.ReadLine();
    }

    private static async Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await ProcedureMessageAsync(update);
    }

    private static async Task ProcedureMessageAsync(Update update)
    {
        if (update.Message != null && update.Message.Entities.Any())
        {
            foreach (var entity in update.Message.Entities)
                await ProcedureCommand(update.Message, entity);
        }
    }

    private static async Task ProcedureCommand (Message message, MessageEntity entity)
    {
        if (entity.Type != MessageEntityType.BotCommand)
            await _botClient.SendStickerAsync(
                chatId: message.Chat.Id,
                sticker: errorSticker
                );
            await _botClient.SendTextMessageAsync(message.Chat, "Введите команду");

        var command = string.Join("", message.Text.Skip(entity.Offset).Take(entity.Length));

        if (!_commandsAvailable.Contains(command))
        {

        }
    }
}
