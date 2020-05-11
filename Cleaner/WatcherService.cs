﻿using System.IO;
using System.Linq;
using Serilog.Events;

namespace Cleaner
{
    public class WatcherService
    {
        private readonly FileSystemWatcher _watcher;
        private readonly CliOptions _options;

        public WatcherService(CliOptions options)
        {
            _options = options;
            var directoryInfo = new DirectoryInfo(_options.Directory);
            _watcher = new FileSystemWatcher(directoryInfo.FullName, _options.Pattern)
            {
                IncludeSubdirectories = _options.IncludeSubDirectories,
                NotifyFilter = NotifyFilters.LastWrite
            };
            InitialClean();
            _watcher.Created += OnFileCreated;
            _watcher.Changed += OnFileChanged;
            Logger.Log("Watcher initialized. Watching {directory}. Including subfolders: {include}",
                LogEventLevel.Information,
                directoryInfo.FullName,
                _watcher.IncludeSubdirectories);
        }

        private void InitialClean()
        {
            Logger.Log("Executing initial clean up.");
            var directoryInfo = new DirectoryInfo(_options.Directory);
            var files = Directory.GetFiles(directoryInfo.FullName, _options.Pattern);
            foreach (var file in files) CleanFile(file);
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Logger.Log("Detected file change: {file}", LogEventLevel.Debug, e.FullPath);
            CleanFile(e.FullPath);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Logger.Log("Detected file creation: {file}", LogEventLevel.Debug, e.FullPath);
            CleanFile(e.FullPath);
        }

        private void CleanFile(string fileName)
        {
            var tempFileName = $"{fileName}.temp";
            if (File.Exists(tempFileName)) File.Delete(tempFileName);
            File.Copy(fileName, tempFileName);
            try
            {
                var content = File.ReadLines(tempFileName).ToList();
                var filteredContent = content
                    .Where(line => _options.KeepRows
                        ? _options.GetRegexFilters()
                            .Any(regex => regex.IsMatch(line))
                        : _options.GetRegexFilters()
                            .All(regex => !regex.IsMatch(line)))
                    .ToList();
                var removedLines = content.Count - filteredContent.Count;
                Logger.Log("Filtered {filterCount} out of {@overallCount} lines",
                    LogEventLevel.Debug,
                    removedLines,
                    content.Count);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var filterDirectory = new DirectoryInfo(_options.FilterDirectory);
                if (!filterDirectory.Exists) Directory.CreateDirectory(filterDirectory.FullName);
                var filterFile = new FileInfo($"{fileNameWithoutExtension}{_options.Suffix}.txt");
                var combinedFilterFile = Path.Combine(filterDirectory.FullName, filterFile.Name);
                File.WriteAllLines(combinedFilterFile, filteredContent);
                Logger.Log("Filtered {removedLines} lines from {file}.", LogEventLevel.Information, removedLines,
                    fileName);
                Logger.Log("Wrote filtered log to {filteredFile}.", LogEventLevel.Information, combinedFilterFile);
            }
            finally
            {
                if (File.Exists(tempFileName)) File.Delete(tempFileName);
            }
        }

        public void StartWatching()
        {
            _watcher.EnableRaisingEvents = true;
        }
    }
}