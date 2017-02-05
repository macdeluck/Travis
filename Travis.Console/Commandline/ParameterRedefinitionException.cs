using Travis.Logic.Extensions;

namespace Travis.Console.Commandline
{
    /// <summary>
    /// Exception which occurs when parameter has been redefined.
    /// </summary>
    public class ParameterRedefinitionException : CommandlineParseException
    {
        /// <summary>
        /// Initializes new instance of <see cref="ParameterRedefinitionException"/> class.
        /// </summary>
        /// <param name="parameterName">The name of redefined parameter.</param>
        /// <param name="position">The redefinition occurance position.</param>
        public ParameterRedefinitionException(string parameterName, int position)
            : base(CommandlineMessages.ParameterRedefinitionMessage
                  .FormatString(parameterName, position))
        {
        }
    }
}
