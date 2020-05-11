﻿using System;
using Serilog;
using Serilog.Events;

namespace Cleaner
{
    public static class Logger
    {
        private const string OutputTemplate =
            "[--- {Timestamp:yyyy-MM-dd HH:mm:ss.fff K} [{Level:u3}] ---] {Message:lj}{NewLine}{Exception}";

        private static ILogger _logger;

        public static void Initialize(LogEventLevel logEventLevel)
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(logEventLevel, OutputTemplate)
                .CreateLogger();
        }

        public static void Log(string message, LogEventLevel logEventLevel = LogEventLevel.Information, params object[] objects)
        {
            _logger.Write(logEventLevel, message, objects);
        }

        public static void LogException(Exception exception)
        {
            _logger.Write(LogEventLevel.Error, exception, exception.Message);
        }
    }
}