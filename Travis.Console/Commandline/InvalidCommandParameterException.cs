using Travis.Logic.Extensions;

namespace Travis.Console.Commandline
{
    /// <summary>
    /// Exception which occurs when parameter is invalid for command.
    /// </summary>
    public class InvalidCommandParameterException : CommandlineParseException
    {
        /// <summary>
        /// Initializes new instance of <see cref="InvalidCommandParameterException"/> class.
        /// <param name="commandName">The command which has invalid parameter.</param>
        /// <param name="parameterName">The name of parameter which is unexpected for the command.</param>
        /// <param name="position">The position of invalid parameter occurance.</param>
        /// </summary>
        public InvalidCommandParameterException(string commandName, string parameterName, int position)
            : base(CommandlineMessages.InvalidCommandParameterMessage
                  .FormatString(commandName, parameterName, position))
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="InvalidCommandParameterException"/> class.
        /// <param name="commandName">The command which has invalid parameter.</param>
        /// <param name="parameterName">The name of parameter which is unexpected for the command.</param>
        /// <param name="position">The position of invalid parameter occurance.</param>
        /// <param name="detailMessage">The detailed error description.</param>
        /// </summary>
        public InvalidCommandParameterException(string commandName, string parameterName, 
            int position, string detailMessage)
            : base(CommandlineMessages.InvalidCommandParameterDetailedMessage
                  .FormatString(commandName, parameterName, position, detailMessage))
        {
        }
    }
}
