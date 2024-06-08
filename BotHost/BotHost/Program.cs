using BotAPILib;
using BotHost.DTOs;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static BotHost.DTOs.StickerFactory;

namespace BotHost;

public class Program
{
    private static TelegramBotClient _botClient;

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
        if (update.Message != null) await HandleMessage(update.Message);
        else if (update.CallbackQuery != null) await HandleCallbackQuery(update.CallbackQuery, update.CallbackQuery.Message.Chat);
    }

    private static async Task HandleCallbackQuery(CallbackQuery callbackQuery, Chat chat)
    {
        await (callbackQuery.Data switch
        {
            var data when data.StartsWith("/seeDiscipline_") => HandleDisciplineInfo(data, chat)
        });
    }

    private static async Task HandleMessage(Message message)
    {
        if (message.Entities != null && message.Entities.Any())
        {
            if (message.Entities.Length > 1)
            {
                await _botClient.SendTextMessageAsync(message.Chat, "Не больше одного запроса за раз!");
                await _botClient.SendStickerAsync(message.Chat, GetSticker(DTOs.StickerType.NononoMisterFish));
            }
            else
            {
                await ProcedureCommand(message, message.Entities[0]);
            }
        }
        else if (message != null)
            await ProcedureCommand(message);
    }

    private static async Task ProcedureCommand (Message message, MessageEntity entity = null)
    {
        if (entity == null || entity.Type != MessageEntityType.BotCommand)
        {
            await _botClient.SendStickerAsync(message.Chat.Id, GetSticker(DTOs.StickerType.NotCommand)
                );
            await _botClient.SendTextMessageAsync(message.Chat, "Я тут для команд вообще-то!");
        }

        if (entity != null)
        {
            var command = string.Join("", message.Text.Skip(entity.Offset).Take(entity.Length));

            await (command switch
            {
                "/workers" => HandleWorkers(message.Chat),
                "/disciplines" => HandleDisciplines(message.Chat),
                "/appoint" => _botClient.SendTextMessageAsync(message.Chat, "appoint"),
                "/workersByDiscipline" => Template(message.Chat),
                "/nextAvailableTime" => Template(message.Chat),
                var cmd when cmd == "/start" || cmd == "/menu" => AddMenuButtons(message.Chat),
                _ => NotCommandMessage(message)
            });
        }
    }

    private static async Task AddMenuButtons(Chat chat)
    {
        await AddButtons("Добро пожаловать в меню!", chat, 2, _commandsAvailable);
        await _botClient.SendStickerAsync(chat, GetSticker(DTOs.StickerType.Hello));
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
        await _botClient.SendStickerAsync(message.Chat.Id, GetSticker(DTOs.StickerType.BadCommand));
    }

    #region CallbackQuery
    private static async Task HandleDisciplineInfo(string data, Chat chat)
    {
        var disciplineID = int.Parse(data.Split('_')[1]);
        var res = await CoreRequests.GetWorkersByDiscipline(disciplineID);

        var sb = new StringBuilder("Список доступных исполнителей:\n");
        var result = string.Join(",\n", res.Select(x => x.UserName));
        sb.Append(result);
        
        await _botClient.SendTextMessageAsync(chat, sb.ToString());
    }

    #endregion

    #region Commands Handlers

    private static async Task Template(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat, "В разработке!");
        await _botClient.SendStickerAsync(chat,StickerFactory.GetSticker(DTOs.StickerType.WorkInProgress));
    }

    private static async Task HandleDisciplines(Chat chat)
    {
        
        var disciplines = await CoreRequests.GetDisciplines();

        var buttons = disciplines.Select(x => InlineKeyboardButton.WithCallbackData(x.Name, $"/seeDiscipline_{x.ID}")).ToList();

        var kbrdMarkup = new InlineKeyboardMarkup(buttons);
        await _botClient.SendTextMessageAsync(chat, "Доступные дисциплины:", replyMarkup: kbrdMarkup);
    }

    private static async Task HandleWorkers(Chat chat)
    {
        var workers = await CoreRequests.GetWorkers();

        var sb = new StringBuilder("Доступные исполнители:\n");

        sb.Append(string.Join(",\n", workers.Select(x => x.UserName)));

        await _botClient.SendTextMessageAsync(chat, sb.ToString());
    }

    #endregion
}
