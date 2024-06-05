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
    private static readonly InputFile _notCommandSticker = InputFile.FromString("CAACAgIAAxkBAAEF7QNmYI5H6op8eacWz-5U5QOSnNlB-QACsToAAg-IcEqRrCbJQ4Uv9TUE");
    private static readonly InputFile _badCommandSticker = InputFile.FromString("CAACAgIAAxkBAAEF7QlmYI_dRm2j14b-2m1vRgOFKaBwxwAC4j0AAl-PaUqyeSmkhxNJCDUE");
    private static readonly InputFile _nononoMisterFishSticker = InputFile.FromString("CAACAgIAAxkBAAEF7RtmYJKd7w_ZLJCbyjjSu1HM9HNFEgACbDcAAhBMkEpX9H_SMikrKjUE");

    private static List<string> _commandsAvailable = new()
    {
        "/workers",
        "/disciplines",
        "/appoint",
        "/workersByDiscipline"
    };

    static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        var token = "6752858428:AAF4gAdbnkVzOeYnPlyaqbs02J0vYovFpdE"; //await CoreRequests.GetBotTokenAsync("qwerty");
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
        if (update.Message.Entities != null && update.Message.Entities.Any())
        {
            if (update.Message.Entities.Length > 1)
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat, "Не больше одного запроса за раз!");
                await _botClient.SendStickerAsync(update.Message.Chat, _nononoMisterFishSticker);
            }
            else
            {
                await ProcedureCommand(update.Message, update.Message.Entities[0]);
            }
        }
        else if (update.Message != null)
            await ProcedureCommand(update.Message);
    }

    private static async Task ProcedureCommand (Message message, MessageEntity entity = null)
    {
        if (entity == null || entity.Type != MessageEntityType.BotCommand)
        {
            await _botClient.SendStickerAsync(
                chatId: message.Chat.Id,
                sticker: _notCommandSticker
                );
            await _botClient.SendTextMessageAsync(message.Chat, "Я тут для команд вообще-то!");
        }

        if (entity != null)
        {
            var command = string.Join("", message.Text.Skip(entity.Offset).Take(entity.Length));

            await (command switch
            {
                "/workers" => CoreRequests.GetWorkers(),
                "/disciplines" => _botClient.SendTextMessageAsync(message.Chat, "disciplines"),
                "/appoint" => _botClient.SendTextMessageAsync(message.Chat, "appoint"),
                "/workersByDiscipline" => _botClient.SendTextMessageAsync(message.Chat, "workersByDiscipline"),
                _ => NotCommandMessage(message)
            });
        }
    }

    private static async Task NotCommandMessage(Message message)
    {
        await _botClient.SendTextMessageAsync(message.Chat, "Какая-то хуета, а не команда");
        await _botClient.SendStickerAsync(
            chatId: message.Chat.Id,
            sticker: _badCommandSticker
            );
    }
}
