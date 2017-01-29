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
            cmdParser.Parse(argv);
            switch (cmdParser.ProgramCommand)
            {
                case Command.Unknown:
                case Command.NoCommand:
                case Command.Help:
                    {
                        System.Console.WriteLine(cmdParser.OutputMessage);
                        return 1;
                    }
                case Command.Learn:
                    return 0;
                default:
                    return 255;
            }
        }
    }
}
