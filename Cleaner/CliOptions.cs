﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace Cleaner
{
    public class CliOptions
    {
        [Option('d', "directory", Required = false, Default = ".", HelpText = "The directory to clean.")]
        public string Directory { get; set; }

        [Option('p', "pattern", Required = false, Default = "*.log",
            HelpText = "The regular expression by which the program is going to select the logs to clean.")]
        public string Pattern { get; set; }

        [Option('s', "sub-directories", Required = false, Default = false, HelpText = "Include sub directories.")]
        public bool IncludeSubDirectories { get; set; }

        [Option('f', "filter", Required = false, Default = new[]
        {
            @"\[FileIO \]\s\d+\.\d+\s.+Failed.+signature for file\s",
            @"\[=ERROR=]\s\d+\.\d+\s.+Could not find file\s'.+\.sig.*'",
            @"\[General\]\s\d+\.\d+ ={38}"
        })]
        public IEnumerable<string> Filters { get; set; }

        [Option("filter-directory", Required = false, Default = ".", HelpText = "The directory to store the filtered log files.")]
        public string FilterDirectory { get; set; }
        [Option("suffix", Required = false, Default = "_Filtered", HelpText = "The suffix to mark the log file as filtered. For example, '_Filtered' will turn logfile.log to logfile_Filtered.log")]
        public string Suffix { get; set; }
        [Option("debug", Default = false, HelpText = "Run the application in debug mode.")]
        public bool Debug { get; set; }
        [Option("keep-rows", Default = false, HelpText = "Set this to true to keep the rows matching the pattern you specified, rather than remove them.")]
        public bool KeepRows { get; set; }


        public IEnumerable<Regex> GetRegexFilters()
        {
            return Filters.Select(filter => new Regex(filter));
        }
    }
}
