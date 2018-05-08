using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Common
{
    public static class LogExtensions
    {
        private static string GetExceptionMessage(Exception ex, string module, string message = "")
        {
            if (ex == null)
            {
                return String.Empty;
            }

            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(message))
            {
                sb.Append(message).Append(" - ");
            }

            while (ex != null)
            {
                sb.AppendFormat("Message: {0} - Trace: {1}{2}", ex.Message, ex.StackTrace, Environment.NewLine);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }

        public static void Exception(this ILog log, Exception ex, string module, string message = "")
        {
            if (log == null)
            {
                return;
            }

            log.Error(GetExceptionMessage(ex, module, message));
        }
        public static void LogFormat(this ILog log, Log4NetLevel logLevel, string module, string message = "", params object[] parms)
        {
            if (log == null)
            {
                return;
            }

            string fullMsg = String.Empty;
            if (!String.IsNullOrWhiteSpace(module))
            {
                fullMsg = String.Format("***** {0} ***** - ", module.ToUpper());
            }

            if (!String.IsNullOrWhiteSpace(message))
            {
                if (parms != null && parms.Length > 0)
                {
                    message = String.Format(message, parms);
                }

                fullMsg += message;
            }

            if (String.IsNullOrWhiteSpace(fullMsg))
            {
                return;
            }

            switch (logLevel)
            {
                case Log4NetLevel.Info:
                    log.Info(fullMsg);
                    break;
                case Log4NetLevel.Warn:
                    log.Warn(fullMsg);
                    break;
                case Log4NetLevel.Error:
                    log.Error(fullMsg);
                    break;
                case Log4NetLevel.Fatal:
                    log.Fatal(fullMsg);
                    break;
                case Log4NetLevel.Debug:
                default:
                    log.Debug(fullMsg);
                    break;
            }
        }

        public static void Log(this ILog log, Log4NetLevel logLevel, string module, string message = "")
        {
            LogFormat(log, logLevel, module, message, null);
        }
    }
}
