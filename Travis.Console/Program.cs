using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;

namespace Travis.Console
{
    public class Options
    {
        public const string learn = nameof(learn);
        public const string play = nameof(play);

        [VerbOption(learn, HelpText="Learns MCTS tree")]
        public LearnOptions Learn { get; set; }

        [VerbOption(play, HelpText = "Performs MCTS game")]
        public PlayOptions Play { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
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
            if (subOptions == null)
                return;
            if (verb == Options.learn)
            {
                var options = subOptions as LearnOptions;
                new LearnProgram().Run(options);
            }
            else if (verb == Options.play)
            {
                var options = subOptions as PlayOptions;
                new PlayProgram().Run(options);
            }
        }
    }
}
