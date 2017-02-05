using System.Linq;
using Travis.Logic.Extensions;

namespace Travis.Console.Commandline.Tokens
{
    class LearnCommand : CommandToken
    {
        public override string CommandHelp => TokenMessages.HelpLearn;

        public override string Name => Commands.Learn;
        
        private struct LearnParameterNames
        {
            public const string Game = "game";
            public const string Budget = "budget";
            public const string Selectors = "selectors";
        }

        public override void OnInterpret(CommandlineTokenizer tokenizer, CommandlineContext context)
        {
            context.LearnOptions = new LearnCommandOptions();
            while (tokenizer.Next())
            {
                if (tokenizer.Current is CommandlineParameterToken)
                {
                    var param = tokenizer.Current as CommandlineParameterToken;
                    if (param.IsParameter(LearnParameterNames.Game))
                        HandleGameParameter(tokenizer, context, param);
                    else if (param.IsParameter(LearnParameterNames.Budget))
                        HandleBudgetParameter(tokenizer, context, param);
                    else if (param.IsParameter(LearnParameterNames.Selectors))
                        HandleSelectorsParameter(tokenizer, context, param);
                    else throw new InvalidCommandParameterException(Name, param.Name, tokenizer.CurrentPosition);
                }
                else
                {
                    tokenizer.Abort();
                    throw new SyntaxException(tokenizer.CurrentPosition);
                }
            }
        }

        private void HandleGameParameter(CommandlineTokenizer tokenizer, CommandlineContext context, CommandlineParameterToken param)
        {
            if (context.LearnOptions.GameName.HasValue())
                throw new ParameterRedefinitionException(param.Name, tokenizer.CurrentPosition);

            AssertNoKeyedArgumentsAllowed(tokenizer, param);
            AssertSingleArgumentAllowed(tokenizer, param);

            context.LearnOptions.GameName = param.Arguments.Single();
        }

        private void HandleBudgetParameter(CommandlineTokenizer tokenizer, CommandlineContext context, CommandlineParameterToken param)
        {
            if (context.LearnOptions.BudgetProviderName.HasValue())
                throw new ParameterRedefinitionException(param.Name, tokenizer.CurrentPosition);

            AssertSingleArgumentAllowed(tokenizer, param);

            context.LearnOptions.BudgetProviderName = param.Arguments.Single();
            context.LearnOptions.BudgetArgumentList = param.KeyedArguments;
        }

        private void HandleSelectorsParameter(CommandlineTokenizer tokenizer, CommandlineContext context, CommandlineParameterToken param)
        {
            if (context.LearnOptions.SelectorNames.Any())
                throw new ParameterRedefinitionException(param.Name, tokenizer.CurrentPosition);

            AssertNoKeyedArgumentsAllowed(tokenizer, param);

            context.LearnOptions.SelectorNames = param.Arguments;
        }
    }
}
