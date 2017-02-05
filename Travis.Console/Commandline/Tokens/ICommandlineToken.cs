namespace Travis.Console.Commandline.Tokens
{
    interface ICommandlineToken
    {
        void Init(CommandlineTokenizer tokenizer);

        void Interpret(CommandlineTokenizer tokenizer, CommandlineContext context);
    }
}
