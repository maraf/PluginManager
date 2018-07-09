using System;
using System.IO;

namespace PackageManager.Cli
{
    class Program
    {
        public static Args Args { get; private set; }

        static void Main(string[] args)
        {
            if (ParseParameters(args))
            {

            }
        }

        private static bool ParseParameters(string[] args)
        {
            Args = new Args();

            int skipped = 0;
            if (args[0] == "update")
            {
                skipped++;
                if (args[1] == "--count")
                {
                    Args.IsUpdateCount = true;
                    skipped++;
                }
            }

            if ((args.Length - skipped) % 2 == 0)
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    string name = args[i];
                    string value = args[i + 1];
                    ParseParameter(name, value);
                }
            }

            if (!Directory.Exists(Args.Path))
            {
                Console.WriteLine("Missing argument '--path' - a target path to install packages to.");
                return false;
            }

            if (!Uri.IsWellFormedUriString(Args.PackageSourceUrl, UriKind.Absolute))
            {
                Console.WriteLine("Missing argument '--path' - a target path to install packages to.");
                return false;
            }

            return true;
        }

        private static bool ParseParameter(string name, string value)
        {
            switch (name)
            {
                case "--path":
                    Args.Path = value;
                    return true;
                case "--packagesource":
                    Args.PackageSourceUrl = value;
                    return true;
                default:
                    return false;
            }
        }
    }
}
