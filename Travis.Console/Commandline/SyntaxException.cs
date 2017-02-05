using System;
using Travis.Logic.Extensions;

namespace Travis.Console.Commandline
{
    /// <summary>
    /// Represents commandline syntax error.
    /// </summary>
    public class SyntaxException : CommandlineParseException
    {
        /// <summary>
        /// Position where syntax error occured.
        /// </summary>
        public int CharNum { get; }

        /// <summary>
        /// Creates new instance of <see cref="SyntaxException"/> with default message.
        /// </summary>
        /// <param name="charNum">Position where syntax error occured.</param>
        public SyntaxException(int charNum)
            : base(CommandlineMessages.SyntaxErrorMessage.FormatString(charNum))
        {
            CharNum = charNum;
        }

        /// <summary>
        /// Creates new instance of <see cref="SyntaxException"/> with default message.
        /// </summary>
        /// <param name="charNum">Position where syntax error occured.</param>
        /// <param name="detailMessage">Detail exception message.</param>
        public SyntaxException(int charNum, string detailMessage)
            : base(CommandlineMessages.SyntaxErrorDetailedMessage.FormatString(charNum, detailMessage))
        {
        }
    }
}
