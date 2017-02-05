using Travis.Console.Commandline;
using Travis.Logic.Extensions;

namespace Travis.Console
{
    /// <summary>
    /// Class which contains program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        public static int Main(string[] argv)
        {
            var cmdParser = new CommandlineParser();
            var context = cmdParser.Parse(argv);
            switch (context.Command)
            {
                case Commands.Help:
                    System.Console.WriteLine(context.OutputMessage);
                    return 1;
                case Commands.Learn:
                    new LearnProgram().Run(context.LearnOptions);
                    return 0;
                default:
                    if (context.OutputMessage.HasValue())
                        System.Console.WriteLine(context.OutputMessage);
                    return 255;
            }
        }
    }
}
