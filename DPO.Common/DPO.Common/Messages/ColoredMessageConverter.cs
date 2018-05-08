using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Util;
using log4net.Layout.Pattern;
using System.IO;
using log4net.Core;

namespace DPO.Common
{
    public class ColoredMessageConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            string color = "";
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                    color = "green";
                    break;
                case "WARN":
                    color = "Yellow";
                    break;
                case "INFO":
                    color = "Blue";
                    break;
                case "ERROR":
                    color = "pink";
                    break;
                case "FATAL":
                    color = "red";
                    break;
            }

            string messages = string.Format("<p style=color:{0}> TimeStamp: {1}, Level: {2}, ClassName: {3}," +
                                            "LineNumber: {4}, MethodName: {5}, User: {6}, Message: {7}",
                                            color, loggingEvent.TimeStamp.ToShortTimeString(),
                                            loggingEvent.Level.Name,
                                            loggingEvent.LocationInformation.ClassName,
                                            loggingEvent.LocationInformation.LineNumber,
                                            loggingEvent.LocationInformation.MethodName,
                                            loggingEvent.Identity,
                                            loggingEvent.MessageObject);
            writer.Write(messages);
        }
    }
}
