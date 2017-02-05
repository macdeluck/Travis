namespace Travis.Console.Commandline.Tokens
{
    class HelpCommandToken : CommandToken
    {
        public override string CommandHelp => TokenMessages.HelpMessage;

        public override string Name => Commands.Help;

        public override void OnInterpret(CommandlineTokenizer tokenizer, CommandlineContext context)
        {
            if (tokenizer.Next() && tokenizer.Current is CommandToken)
            {
                context.OutputMessage = (tokenizer.Current as CommandToken).CommandHelp;
            }
            else
            {
                context.OutputMessage = CommandHelp;
            }
            tokenizer.Abort();
        }
    }
}
