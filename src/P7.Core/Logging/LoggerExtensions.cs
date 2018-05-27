using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace P7.Core.Logging
{
    public static class LoggerExtensions
    {
        //
        // Summary:
        //     Formats and writes an error log message.
        //
        // Parameters:
        //   logger:
        //     The Microsoft.Extensions.Logging.ILogger to write to.
        //
        //   eventId:
        //     The event id associated with the log.
        //
        //   exception:
        //     The exception to log.
        //
        //   args:
        //     An object
        public static void LogError(this ILogger logger, EventId eventId, Exception exception, params object[] args)
        {
            logger.LogError(eventId, exception, exception.Message, args);
        }

        //
        // Summary:
        //     Formats and writes a critical log message.
        //
        // Parameters:
        //   logger:
        //     The Microsoft.Extensions.Logging.ILogger to write to.
        //
        //   eventId:
        //     The event id associated with the log.
        //
        //   exception:
        //     The exception to log.
        //
        //   args:
        //     An object array that contains zero or more objects to format.
        public static void LogCritical(this ILogger logger, EventId eventId, Exception exception, params object[] args)
        {
            logger.LogCritical(eventId,exception,exception.Message,args);
        }

    }
}
