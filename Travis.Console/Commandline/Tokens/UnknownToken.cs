using System;

namespace Travis.Console.Commandline.Tokens
{
    class UnknownToken : ICommandlineToken
    {
        public void Init(CommandlineTokenizer tokenizer)
        {
            throw new SyntaxException(tokenizer.CurrentPosition);
        }

        public void Interpret(CommandlineTokenizer tokenizer, CommandlineContext context)
        {
            throw new SyntaxException(tokenizer.CurrentPosition);
        }
    }
}
