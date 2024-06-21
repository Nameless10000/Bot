using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BotLogger
{
    public class LoggerLib : IDisposable
    {
        private StreamWriter _logWriter;

        public void ConfigureLogger(StreamWriter? logTo = null)
        {
            _logWriter = logTo ?? new (Console.OpenStandardOutput());
        }

        public async Task Error(Exception ex)
        {
            var msg = CreateMessage(MessageLevel.Error, ex.Message);
            await WriteLog(msg);
        }

        public async Task Error(string message)
        {
            var msg = CreateMessage(MessageLevel.Error, message);
            await WriteLog(msg);
        }

        public async Task Warning(string message)
        {
            var msg = CreateMessage(MessageLevel.Warning, message);
            await WriteLog(msg);
        }

        public async Task Info(string message)
        {
            var msg = CreateMessage(MessageLevel.Info, message);
            await WriteLog(msg);
        }

        private string CreateMessage(MessageLevel messageLevel, string message)
        {
            return $"[{messageLevel}] - [{DateTime.Now:g}]: {message}";
        }

        private async Task WriteLog(string log)
        {
            lock(_logWriter)
            {
                _logWriter.WriteLineAsync(log).GetAwaiter().GetResult();
                _logWriter.FlushAsync().GetAwaiter().GetResult(); 
            }
        }

        public void Dispose()
        {
            _logWriter?.Dispose();
        }

        private enum MessageLevel
        {
            Info,
            Warning,
            Error
        }
    }
}
