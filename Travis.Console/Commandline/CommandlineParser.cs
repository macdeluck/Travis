namespace Travis.Console.Commandline
{
    /// <summary>
    /// Parser of commandline arguments.
    /// </summary>
    public class CommandlineParser
    {
        /// <summary>
        /// Parses array of arguments.
        /// </summary>
        /// <param name="arguments">Array of program arguments.</param>
        public CommandlineContext Parse(string[] arguments)
        {
            var tokenizer = new CommandlineTokenizer(arguments);
            var context = new CommandlineContext();
            try
            {
                while (tokenizer.Next())
                    tokenizer.Current.Interpret(tokenizer, context);
            }
            catch (CommandlineParseException exc)
            {
                tokenizer.Abort();
                context.Command = null;
                context.OutputMessage = exc.Message;
            }
            return context;
        }
    }
}
