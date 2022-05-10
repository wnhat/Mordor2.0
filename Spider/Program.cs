namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            // get parameter from args;
            SpiderParameter.initialize(args);
            Run();
        }
        static void Run()
        {
            Spider.Run();
        }
    }
    public static class SpiderParameter
    {
        public static string Pcip;
        internal static void initialize(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { "172.16.200.100" };
            }
            // "@tcp://172.16.210.22:5554";
            Pcip = @"@tcp://" + args[0] + ":5554";
        }
    }
}
