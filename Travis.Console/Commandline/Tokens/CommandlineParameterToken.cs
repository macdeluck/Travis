using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;

namespace Travis.Console.Commandline.Tokens
{
    class CommandlineParameterToken : ICommandlineToken
    {
        public const string CommandlineParameterPrefix = "/";

        private const char ParameterNameAndArgumentsSeparator = ':';

        private const char ParameterArgumentsSeparator = ',';

        private const char KeyedParameterArgumentSeparator = '=';

        private int currentPosition;

        private string rawArgument;

        public string Name { get; private set; }

        public IList<string> Arguments { get; private set; }

        public IList<KeyValuePair<string, string>> KeyedArguments { get; private set; }
        
        private void ParseParameter()
        {
            if (!rawArgument.StartsWith(CommandlineParameterPrefix))
                throw new SyntaxException(currentPosition, 
                    TokenMessages.CommandlineParameterInvalidPrefix.FormatString(CommandlineParameterPrefix));
            currentPosition++;
            var nameAndArgs = rawArgument.Split(ParameterNameAndArgumentsSeparator);
            if (nameAndArgs.Length < 1 || !nameAndArgs[0].HasValue())
                throw new SyntaxException(currentPosition, TokenMessages.CommandlineParameterNoName);
            if (nameAndArgs.Length > 2)
                throw new SyntaxException(currentPosition + nameAndArgs[0].Length + 1 + nameAndArgs[1].Length, TokenMessages.UnexpectedCharacter.FormatString(ParameterNameAndArgumentsSeparator));
            Name = nameAndArgs[0].Trim().Substring(CommandlineParameterPrefix.Length);
            ParseParameterArguments(nameAndArgs[1]);
        }

        private void ParseParameterArguments(string parameterArguments)
        {
            var args = parameterArguments.Split(ParameterArgumentsSeparator);
            Arguments = new List<string>();
            KeyedArguments = new List<KeyValuePair<string, string>>();
            foreach (var arg in args)
            {
                ParseParameterArgument(arg);
                currentPosition = arg.Length + 1;
            }
        }

        private void ParseParameterArgument(string argument)
        {
            var keyAndValue = argument.Split(KeyedParameterArgumentSeparator);
            if (keyAndValue.Length == 1)
                Arguments.Add(keyAndValue[0]);
            else if (keyAndValue.Length == 2)
            {
                var key = keyAndValue[0];
                var value = keyAndValue[1];
                if (KeyedArguments.Any(kv => kv.Key.Equals(key)))
                    throw new SyntaxException(currentPosition,
                        TokenMessages.CommandlineParameterArgumentDefinedMultipleTimes.FormatString(key));
                KeyedArguments.Add(new KeyValuePair<string, string>(key, value));
            }
            else throw new SyntaxException(currentPosition + keyAndValue[0].Length + 1 + keyAndValue[1].Length,
                TokenMessages.UnexpectedCharacter.FormatString(KeyedParameterArgumentSeparator));
        }

        public void Interpret(CommandlineTokenizer tokenizer, CommandlineContext context)
        {
        }

        public void Init(CommandlineTokenizer tokenizer)
        {
            currentPosition = tokenizer.CurrentPosition;
            rawArgument = tokenizer.CurrentArg;
            ParseParameter();
        }
        
        public bool IsParameter(string name)
        {
            return string.Equals(Name, name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
