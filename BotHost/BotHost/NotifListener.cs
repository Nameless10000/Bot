using System.Net;
using System.Text;
using Telegram.Bot;
using BotLogger;
using Newtonsoft.Json;
using BotApi.Models.DTOs;

namespace BotHost;

public class NotifListener (TelegramBotClient _botClient, LoggerLib _logger)
{
    public async Task StartServer(string url, CancellationToken cancellationToken)
    {
        await Task.Run(async () =>
        {
                using HttpListener listener = new();
            try
            {
                listener.Prefixes.Add(url);
                listener.Start();
            }
            catch(Exception ex) { }

            await _logger.Info($"Listening {url}");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Ожидание входящего запроса
                    var context = await listener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;

                    // Обработка запроса
                    using var sr = new StreamReader(request.InputStream);
                    var stringifiedNotifs = await sr.ReadLineAsync();
                    var notifications = JsonConvert.DeserializeObject<List<NotificationDTO>>(stringifiedNotifs);

                    foreach (var notification in notifications)
                    {
                        await _botClient.SendTextMessageAsync(notification.ChatID, notification.Message);
                    }

                    await _logger.Info($"[{DateTime.Now}] Request received: {request.HttpMethod} {request.Url}");
                }
                catch (HttpListenerException ex) when (ex.ErrorCode == 995) // Код 995: Операция прервана
                {
                    // Это нормально при отмене
                    Console.WriteLine("Сервер остановлен.");
                    break;
                }
                catch (Exception e)
                {
                    // Обработка всех других исключений
                    Console.WriteLine($"Произошла ошибка: {e.Message}");
                }
            }

            listener.Stop();
        }, cancellationToken);
    }
}