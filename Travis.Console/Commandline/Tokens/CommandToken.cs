using System;
using System.Linq;

namespace Travis.Console.Commandline.Tokens
{
    abstract class CommandToken : ICommandlineToken
    {
        protected int CurrentPosition { get; set; }

        public abstract string Name { get; }
        
        public void Init(CommandlineTokenizer tokenizer)
        {
            CurrentPosition = tokenizer.CurrentPosition;
            AfterInit(tokenizer);
        }

        protected virtual void AfterInit(CommandlineTokenizer tokenizer)
        {
        }
        
        protected void AssertNoKeyedArgumentsAllowed(CommandlineTokenizer tokenizer, CommandlineParameterToken param)
        {
            if (param.KeyedArguments.Any())
                throw new InvalidCommandParameterException(Name, param.Name, tokenizer.CurrentPosition, CommandlineMessages.ParameterNoKeyedArgumentsAllowed);
        }

        protected void AssertSingleArgumentAllowed(CommandlineTokenizer tokenizer, CommandlineParameterToken param)
        {
            if (!param.Arguments.Any() || param.Arguments.Skip(1).Any())
                throw new InvalidCommandParameterException(Name, param.Name, tokenizer.CurrentPosition, CommandlineMessages.ParameterSingleArgumentAllowed);
        }

        public abstract string CommandHelp { get; }

        public abstract void OnInterpret(CommandlineTokenizer tokenizer, CommandlineContext context);

        public void Interpret(CommandlineTokenizer tokenizer, CommandlineContext context)
        {
            context.Command = Name;
            OnInterpret(tokenizer, context);
        }
    }
}
