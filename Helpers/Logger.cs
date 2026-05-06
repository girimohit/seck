using System.IO;

namespace SecureAppLocker.Helpers
{
    public static class Logger
    {
        private static readonly string LogFile = "activity.log";

        public static void Log(string message)
        {
            string log =
                $"[{DateTime.Now}] {message}{Environment.NewLine}";

            File.AppendAllText(LogFile, log);
        }
    }
}