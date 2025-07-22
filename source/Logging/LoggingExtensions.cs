using Microsoft.Extensions.Logging;

namespace WebWacker.Logging
{
    internal static class LoggingExtensions
    {
        public static void LogInformation(this ILogger logger, LogColor color, string message, params object[] args)
        {
            using (NLog.ScopeContext.PushProperty("Color", color.ToString()))
            {
                logger.LogInformation(message, args);
            }
        }
    }
}
