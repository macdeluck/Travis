using System;

namespace Travis.Console
{
    /// <summary>
    /// Console program commands.
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// Unknown console command.
        /// </summary>
        Unknown,

        /// <summary>
        /// No console command has been specified.
        /// </summary>
        NoCommand,

        /// <summary>
        /// Show program help.
        /// </summary>
        Help,

        /// <summary>
        /// Learn game tree.
        /// </summary>
        Learn
    }

    /// <summary>
    /// Stores options for learn command.
    /// </summary>
    public class LearnCommandOptions
    {

    }

    /// <summary>
    /// Parser of console commandline.
    /// </summary>
    public class CommandlineParser
    {
        /// <summary>
        /// Stores program command.
        /// </summary>
        public Command ProgramCommand { get; set; } = Command.Unknown;

        /// <summary>
        /// Parsed argument vector.
        /// </summary>
        public string[] ArgVector { get; private set; } = null;

        /// <summary>
        /// Output message for several commands.
        /// </summary>
        public string OutputMessage { get; set; } = null;

        private int TokenIndex { get; set; } = 0;

        /// <summary>
        /// Parses commandline arguments to program data.
        /// </summary>
        /// <param name="argv">Commandline arguments.</param>
        public void Parse(string[] argv)
        {
            ArgVector = argv;
            TokenIndex = 0;
            ProgramCommand = ParseCommand();
        }

        private Command ParseCommand()
        {
            Command outCmd;
            if (ArgVector.Length <= TokenIndex)
            {
                outCmd = Command.NoCommand;
            }
            else
            {
                if (!Enum.TryParse(ArgVector[TokenIndex], true, out outCmd))
                    outCmd = Command.Unknown;
            }
            switch (outCmd)
            {
                case Command.NoCommand:
                case Command.Help:
                    if (TokenIndex == 0)
                        ParseHelp();
                    break;
                case Command.Learn:
                    ParseLearn();
                    break;
                default:
                    ParseUnknown();
                    break;
            }
            return outCmd;
        }

        private void ParseHelp()
        {
            TokenIndex++;
            var cmd = ArgVector.Length > TokenIndex ? ParseCommand() : Command.NoCommand;
            switch (cmd)
            {
                case Command.NoCommand:
                case Command.Help:
                    OutputMessage = Messages.HelpMessage;
                    break;
                case Command.Learn:
                    OutputMessage = Messages.HelpLearn;
                    break;
                default:
                    break;
            }
        }

        private void ParseUnknown()
        {
            OutputMessage = string.Format(Messages.UnknownCommand, ArgVector[TokenIndex], Messages.HelpMessage);
        }

        private void ParseLearn()
        {
        }
    }
}