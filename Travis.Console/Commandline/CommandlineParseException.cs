using System;

namespace Travis.Console.Commandline
{
    /// <summary>
    /// Base class for all commandline parse exceptions.
    /// </summary>
    public abstract class CommandlineParseException : Exception
    {
        /// <summary>
        /// Initializes new instance of <see cref="CommandlineParseException"/> class.
        /// </summary>
        public CommandlineParseException() : base()
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="CommandlineParseException"/>
        /// class with specified error message.
        /// <param name="message">The message that describes the error.</param>
        /// </summary>
        public CommandlineParseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="CommandlineParseException"/>
        /// class with specified error message and a reference to inner exception that is cause of the error.
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception which is cause of the error.</param>
        /// </summary>
        public CommandlineParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
