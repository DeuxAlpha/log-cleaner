﻿using System;
using CommandLine;
using Serilog.Events;

namespace Cleaner
{
    internal static class Program
    {
        private static bool _initialized;
        private static bool _cancelled;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += OnConsoleCloseKeyPressed;

            Parser.Default.ParseArguments<CliOptions>(args).WithParsed(options =>
            {
                Logger.Initialize(options.Debug ? LogEventLevel.Verbose : LogEventLevel.Information);
                var watcherService = new WatcherService(options);
                while (!_cancelled)
                {
                    if (_initialized)
                    {
                        continue;
                    }
                    watcherService.StartWatching();
                    Logger.Log("Watcher is running. Press Ctrl + C to close the application.");
                    _initialized = true;
                }

                Logger.Log("Process was exited by the user.");
            });
        }

        private static void OnConsoleCloseKeyPressed(object sender, ConsoleCancelEventArgs e)
        {
            _cancelled = true;
        }
    }
}