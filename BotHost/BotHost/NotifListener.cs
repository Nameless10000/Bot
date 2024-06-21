using System.Net;
using System.Text;
using Telegram.Bot;
using BotLogger;

namespace BotHost;

public class NotifListener (TelegramBotClient _botClient, LoggerLib _logger)
{
    Task StartServer(string url, CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            using HttpListener listener = new();
            listener.Prefixes.Add(url);
            listener.Start();

            Console.WriteLine($"Слушаем {url}");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Ожидание входящего запроса
                    var context = await listener.GetContextAsync();
                    var request = context.Request;
                    var response = context.Response;

                    // Обработка запроса
                    string responseString = "Сообщение получено!";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;


                    using var responseStream = response.OutputStream;
                    await responseStream.WriteAsync(buffer, 0, buffer.Length);

                    await _logger.Info($"[{DateTime.Now}] Получен запрос: {request.HttpMethod} {request.Url}");
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