using BotAPILib;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BotLogger;
using BotApi.Models.DTOs;
using static BotHost.StickerFactory;
using static System.Math;
using BotApi.Models.DbEntities;

namespace BotHost;

public class Program
{
    private const int LAST_APPOINTMENT_HOUR = 23;
    private const int FIRST_APPOINTMENT_HOUR = 10;
    private static TelegramBotClient _botClient;
    private static LoggerLib _logger;

    private static List<string> _commandsAvailable = new()
    {
        "/menu",
        "Дисциплины",
        "Мои записи",
        "Меню"
    };

    private static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        //Console.InputEncoding = Encoding.ASCII; Сделать корректный вывод кириллицы в консоль

        _logger = new();
        _logger.ConfigureLogger();

        var token = await CoreRequests.GetBotTokenAsync("qwerty");
        _botClient = new TelegramBotClient(token);
        _botClient.StartReceiving(UpdateHandler, ErrorHandler);
        await _logger.Info($"Bot launched");
        Console.ReadLine();

        _logger.Dispose(); // Высвобождение ресурсов логгера
    }

    private static async Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
       await _logger.Error(exception);
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await _logger.Info(update switch
        {
            var upd when upd.CallbackQuery != null => $"Recieved callback from {update.CallbackQuery.From} : {update.CallbackQuery.Data}",
            var upd when upd.Message != null && !string.IsNullOrWhiteSpace(upd.Message.Text) => $"Recieved message from {update.Message.From} : {update.Message.Text}",
            _ => "unknown update format"
        });

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
            var data when data.StartsWith("/seeDiscipline_") => HandleWorkersWithTime(data, chat),
            var data when data.StartsWith("/selectWorker_") => HandleTimePicker(data, chat),
            var data when data.StartsWith("/selectedTime_") => HandleAppoint(data, chat),
            var data when data.StartsWith("/appointmentDetals_") => HandleAppointmentDetals(data, chat),
            var data when data.StartsWith("/cancelAppointment_") => HandleCancelAppointment(data, chat)
        });
    }

    private static async Task HandleMessage(Message message)
    {
        if (message.Entities != null && message.Entities.Any())
        {
            if (message.Entities.Length > 1)
            {
                await _botClient.SendTextMessageAsync(message.Chat, "Не больше одного запроса за раз!");
                await _botClient.SendStickerAsync(message.Chat, GetSticker(StickerType.NononoMisterFish));
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
        if (message.Text == null || !_commandsAvailable.Contains(message.Text))
        {
            await _botClient.SendStickerAsync(message.Chat.Id, GetSticker(StickerType.NotCommand)
                );
            await _botClient.SendTextMessageAsync(message.Chat, "Я тут для команд вообще-то!");
        }

        var command = _commandsAvailable.FirstOrDefault(x => x == message.Text);

        if (command != null)
        {
            await (command.ToLower().Trim() switch
            {
                "дисциплины" => HandleDisciplines(message.Chat),
                "/start" => HandleAddingUser(message.Chat),
                "/menu" => AddMenuButtons(message.Chat),
                "меню" => AddMenuButtons(message.Chat),
                "мои записи" => HandleSeeAppointments(message.Chat),
                "/cancelAppointment" => Template(message.Chat),
                _ => NotCommandMessage(message)
            });
        }
    }

    private static async Task AddMenuButtons(Chat chat)
    {
        await AddButtons("Добро пожаловать в меню!", chat, 2, _commandsAvailable.Where(x => !x.Contains("/")));
        await _botClient.SendStickerAsync(chat, GetSticker(StickerType.Hello));
    }

    private static async Task AddButtons(string message, Chat chat, int rowsCount, IEnumerable<string> cmds)
    {
        var btns = cmds.Select(x => new KeyboardButton(x));
        var btnsCount = btns.Count();
        var btnsRows = new List<IEnumerable<KeyboardButton>>();

        for (var i = 0; i < rowsCount; i++)
        {
            var iPart = i==rowsCount-1 ? btns.Skip(btnsCount * i / rowsCount).TakeWhile(x => true) : btns.Skip(btnsCount * i / rowsCount).Take(btnsCount / rowsCount);
            btnsRows.Add(iPart);
        }

        var kbrdMarkup = new ReplyKeyboardMarkup(btnsRows);
        await _botClient.SendTextMessageAsync(chat, message, replyMarkup: kbrdMarkup);
    }

    private static async Task NotCommandMessage(Message message)
    {
        await _botClient.SendTextMessageAsync(message.Chat, "Какая-то хуета, а не команда");
        await _botClient.SendStickerAsync(message.Chat.Id, GetSticker(StickerType.BadCommand));
    }

    private static async Task<InlineKeyboardMarkup> GenerateTimeButtons(int from, int to, long workerID, string disciplineID)
    {
        var timeList = new List<List<InlineKeyboardButton>>();
        for (var hour = from; hour<=to;hour++)
        {
            var mph = new List<InlineKeyboardButton>();
            for (var minute = 0; minute < 60; minute += 15)
            {
                mph.Add(InlineKeyboardButton
                    .WithCallbackData($"{hour}:{(minute.ToString()=="0" ? "00" : minute)}",$"/selectedTime_{hour}_{minute}_{workerID}_{disciplineID}"));
            }
            timeList.Add(mph);
        }
        var kbrdMarkup = new InlineKeyboardMarkup(timeList);
        return kbrdMarkup;
    }

    private static async Task<InlineKeyboardMarkup> GenerateAppointmentsButtons(List<Appointment> appointments)
    {
        var buttons = new List<InlineKeyboardButton>();
        foreach (var a in appointments)
        {
            buttons.Add(InlineKeyboardButton
                .WithCallbackData($"{a.Discipline.Name} в {a.StartsAt:g}", $"/appointmentDetals_{a.ID}"));
        }
        var kbrdMarkup = new InlineKeyboardMarkup(buttons);
        return kbrdMarkup;
    }

    #region CallbackQuery
    private static async Task HandleAppointmentDetals(string data, Chat chat)
    {
        var splittedData = data.Split('_');
        var appointmentID = int.Parse(splittedData[1]);

        var appointment = (await CoreRequests.GetAppointments(chat.Id))
            .FirstOrDefault(x => x.ID == appointmentID);

        var message = $"Информация о записи: \nДисциплина: {appointment.Discipline.Name} \nИсполнитель: {appointment.Worker.UserName} \nВремя начала: {appointment.StartsAt:g} \nПродолжительность: {appointment.Longevity}\nСтоимость: {Round(appointment.Price)}";
        var cancelButton = new InlineKeyboardMarkup(InlineKeyboardButton
                .WithCallbackData("Отменить запись", $"/cancelAppointment_{appointmentID}"));

        await _botClient.SendTextMessageAsync(chat, message, replyMarkup: cancelButton);
    }

    private static async Task HandleCancelAppointment(string data, Chat chat)
    {
        var splittedData = data.Split('_');
        var appointmentID = int.Parse(splittedData[1]);

        var res = await CoreRequests.DeleteAppointmentAsync(appointmentID);

        await _botClient.SendTextMessageAsync(chat, res ? "Запись отменена" : "Ошибка отмены записи");
        await _botClient.SendStickerAsync(chat, res ? GetSticker(StickerType.AppointmentCancelSuccess) : GetSticker(StickerType.AppointmentCancelError));
    }

    private static async Task HandleWorkersWithTime(string data, Chat chat)
    {
        int disciplineID = int.Parse(data.Split('_')[1]);
        var workersWithTimes = await CoreRequests.GetNextAvailableTime(disciplineID);

        var buttons = workersWithTimes
            .Select(x => InlineKeyboardButton
                .WithCallbackData($"{x.Worker.UserName} {x.NextAvailableTime:g}", $"/selectWorker_{x.Worker.ID}_{x.Worker.UserName}_{disciplineID}"))
            .ToList();
        var kbrdMarkup = new InlineKeyboardMarkup(buttons);

        await _botClient.SendTextMessageAsync(chat, "Выберите работника для бронирования занятия: \n", replyMarkup: kbrdMarkup);
    }

    private static async Task HandleTimePicker(string data, Chat chat)
    {
        var splittedData = data.Split('_');
        var workerID = int.Parse(splittedData[1]);
        var workerName = splittedData[2];

        var currentHour = DateTime.Now.Hour > FIRST_APPOINTMENT_HOUR 
            ? DateTime.Now.Hour 
            : FIRST_APPOINTMENT_HOUR;

        var kbrdMarkup = await GenerateTimeButtons(currentHour + 1, LAST_APPOINTMENT_HOUR, workerID, splittedData[3]);
        await _botClient.SendTextMessageAsync(chat, currentHour < LAST_APPOINTMENT_HOUR ? $"Выбран исполнитель {workerName}, выберите время:\n" : "Уже поздно, запишитесь завтра", replyMarkup: currentHour < LAST_APPOINTMENT_HOUR ? kbrdMarkup : null);
        await _botClient.SendStickerAsync(chat, GetSticker(currentHour < LAST_APPOINTMENT_HOUR ? StickerType.HasTimeToAppoint : StickerType.NoTimeToAppoint));
    }

    private static async Task HandleAppoint(string data, Chat chat)
    {
        var splittedData = data.Split('_');
        var hour = int.Parse(splittedData[1]);
        var minute = int.Parse(splittedData[2]);
        var workerID = int.Parse(splittedData[3]);
        var disciplineID = int.Parse(splittedData[4]);

        var now = DateTime.Now;
        var selectedTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

        AppointmentCreatingDTO dto = new AppointmentCreatingDTO()
        {
            Longevity = TimeSpan.FromHours(1),
            StartsAt = selectedTime,
            WorkerID = workerID,
            UserID = chat.Id,
            DisciplineID = disciplineID
        };

        var a = await CoreRequests.AppointToWorkerAsync(dto);
        await _botClient.SendTextMessageAsync(chat, a ? "Время успешно забронировано" : "Ошибка - время забронировано");
        await _botClient.SendStickerAsync(chat, a ? GetSticker(StickerType.AppointmentSuccess) : GetSticker(StickerType.AppointmentFailure));
        
    }
    #endregion

    #region Commands Handlers
    private static async Task HandleSeeAppointments(Chat chat)
    {
        var appointments = await CoreRequests.GetAppointments(chat.Id);

        if (appointments == null || appointments.Count == 0)
        {
            await _botClient.SendTextMessageAsync(chat, "У вас нет ни одной записи!");
            await _botClient.SendStickerAsync(chat, GetSticker(StickerType.GotAppointmentsNo));
        }
        else
        {
            var kbrdMarkup = await GenerateAppointmentsButtons(appointments);

            await _botClient.SendTextMessageAsync(chat, "Ваши записи:\n", replyMarkup: kbrdMarkup);
            await _botClient.SendStickerAsync(chat, GetSticker(StickerType.GotAppointmentsAny));
        }
    }

    private static async Task HandleAddingUser(Chat chat)
    {
        var res = await CoreRequests.AddUser(chat.Id, $"{chat.Username}");

        await AddMenuButtons(chat);

        await _botClient.SendTextMessageAsync(chat, res ? "Пользователь успешно зарегестрирован" : "Ошибка регистрации");
        await _botClient.SendStickerAsync(chat, res ? GetSticker(StickerType.RegistrationSuccess) : GetSticker(StickerType.RegistrationFailure));
    }

    private static async Task Template(Chat chat)
    {
        await _botClient.SendTextMessageAsync(chat, "В разработке!");
        await _botClient.SendStickerAsync(chat, GetSticker(StickerType.WorkInProgress));
    }

    private static async Task HandleDisciplines(Chat chat)
    {
        
        var disciplines = await CoreRequests.GetDisciplines();

        var buttons = disciplines.Select(x => InlineKeyboardButton.WithCallbackData(x.Name, $"/seeDiscipline_{x.ID}")).ToList();

        var kbrdMarkup = new InlineKeyboardMarkup(buttons);
        await _botClient.SendTextMessageAsync(chat, "Доступные дисциплины:", replyMarkup: kbrdMarkup);
        await _botClient.SendStickerAsync(chat, GetSticker(StickerType.Disciplines));
    }

    #endregion
}
