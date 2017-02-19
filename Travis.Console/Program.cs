using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;

namespace Travis.Console
{
    public class Options
    {
        public const string learn = nameof(learn);

        [VerbOption(learn)]
        public LearnOptions Learn { get; set; }
    }

    /// <summary>
    /// Class which contains program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        public static void Main(string[] argv)
        {
            if (!Parser.Default.ParseArguments(argv, new Options(), DispatchProgram))
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
        }

        private static void DispatchProgram(string verb, object subOptions)
        {
            if (verb == Options.learn)
            {
                var options = subOptions as LearnOptions;
                new LearnProgram().Run(options);
            }
        }
    }
}
