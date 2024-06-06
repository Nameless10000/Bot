using BotAPILib;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotHost;

public class Program
{
    private static TelegramBotClient _botClient;
    private static readonly InputFile _notCommandSticker = InputFile.FromString("CAACAgIAAxkBAAEF7QNmYI5H6op8eacWz-5U5QOSnNlB-QACsToAAg-IcEqRrCbJQ4Uv9TUE");
    private static readonly InputFile _badCommandSticker = InputFile.FromString("CAACAgIAAxkBAAEF7QlmYI_dRm2j14b-2m1vRgOFKaBwxwAC4j0AAl-PaUqyeSmkhxNJCDUE");
    private static readonly InputFile _nononoMisterFishSticker = InputFile.FromString("CAACAgIAAxkBAAEF7RtmYJKd7w_ZLJCbyjjSu1HM9HNFEgACbDcAAhBMkEpX9H_SMikrKjUE");
    private static readonly InputFile _hello = InputFile.FromString("CAACAgIAAxkBAAEF7aZmYK-BDiV_pQahDj5OxOIlJRQulQACxjAAAluPkEqow5iNYORvuTUE");
    private static readonly InputFile _workInProgress = InputFile.FromString("CAACAgIAAxkBAAEF7cBmYLMUFbjj-qDvJoOVzzxXOIDrJQAChEMAAp4daEqVzKX5VFmspjUE");

    private static List<string> _commandsAvailable = new()
    {
        "/workers",
        "/disciplines",
        "/appoint",
        "/workersByDiscipline",
        "/nextAvailableTime",
        "/menu"
    };

    static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        var token = "6752858428:AAF4gAdbnkVzOeYnPlyaqbs02J0vYovFpdE"; //await GetBotTokenAsync("qwerty");
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
                "/workers" => SendWorkers(message.Chat),
                "/disciplines" => HandleDisciplines(message.Chat),
                "/appoint" => _botClient.SendTextMessageAsync(message.Chat, "appoint"),
                "/workersByDiscipline" => SendWorkers(message.Chat),
                "/nextAvailableTime" => SendWorkers(message.Chat),
                var cmd when cmd == "/start" || cmd == "/menu" => AddMenuButtons(message.Chat),
                _ => NotCommandMessage(message)
            });
        }
    }

    private static async Task AddMenuButtons(Chat chat)
    {
        await AddButtons("Добро пожаловать в меню!", chat, 2, _commandsAvailable);
        await _botClient.SendStickerAsync(chat, _hello);
    }

    private static async Task AddButtons(string message, Chat chat, int rowsCount, IEnumerable<string> cmds)
    {
        var btns = cmds.Select(x => new KeyboardButton(x));
        var btnsCount = btns.Count();
        var btnsRows = new List<IEnumerable<KeyboardButton>>();

        for (var i = 0; i < rowsCount; i++)
        {
            var iPart = btns.Skip(btnsCount * i / rowsCount).Take(btnsCount / rowsCount);
            btnsRows.Add(iPart);
        }

        var kbrdMarkup = new ReplyKeyboardMarkup(btnsRows);
        await _botClient.SendTextMessageAsync(chat, message, replyMarkup: kbrdMarkup);
    }

    private static async Task NotCommandMessage(Message message)
    {
        await _botClient.SendTextMessageAsync(message.Chat, "Какая-то хуета, а не команда");
        await _botClient.SendStickerAsync(
            chatId: message.Chat.Id,
            sticker: _badCommandSticker
            );
    }

    #region Commands Handlers

    private static async Task SendWorkers(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat, "В разработке");
        await _botClient.SendStickerAsync(chat, _workInProgress);
    }

    private static async Task HandleDisciplines(Chat chat)
    {
        var disciplines = await CoreRequests.GetDisciplines();

        var kbrdMarkup = new InlineKeyboardMarkup(disciplines.Select(x => new InlineKeyboardButton(x.Name)));
        await _botClient.SendTextMessageAsync(chat, "Доступные дисциплины:", replyMarkup: kbrdMarkup);
    }

    #endregion
}
