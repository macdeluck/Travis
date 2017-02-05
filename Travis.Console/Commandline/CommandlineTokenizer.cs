using System;
using Travis.Console.Commandline.Tokens;

namespace Travis.Console.Commandline
{
    class CommandlineTokenizer
    {
        public CommandlineTokenizer(string[] arguments)
        {
            ArgVector = arguments;
            currentArg = -1;
            CurrentPosition = 0;
        }

        private int currentArg = -1;

        private string[] ArgVector { get; set; } = null;

        public int CurrentPosition { get; private set; } = 0;

        public ICommandlineToken Current { get; private set; } = null;

        public bool Next()
        {
            if (++currentArg >= ArgVector.Length)
                return false;
            CurrentPosition += CurrentArg.Length + 1;
            Current = GetToken();
            Current.Init(this);
            return true;
        }

        public void Abort()
        {
            currentArg = ArgVector.Length - 1;
        }

        private ICommandlineToken GetToken()
        {
            if (CurrentArg.StartsWith(CommandlineParameterToken.CommandlineParameterPrefix))
                return new CommandlineParameterToken();
            else if (IsCommand<HelpCommandToken>())
                return new HelpCommandToken();
            else if (IsCommand<LearnCommand>())
                return new LearnCommand();
            else return new UnknownToken();
        }

        private bool IsCommand<TCommandToken>()
            where TCommandToken : CommandToken, new()
        {
            return string.Equals(CurrentArg, new TCommandToken().Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public string CurrentArg
        {
            get
            {
                return ArgVector[currentArg].Trim();
            }
        }
    }
}
