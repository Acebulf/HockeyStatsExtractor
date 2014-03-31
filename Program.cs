using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsExtractorConsole2
{
    class Program
    {
        public const int REFRESH_TIME = 75; //Def 50;
        public const int LONG_INTERVAL = 100; //This will warn the user if a long interval is detected.

        public const bool SERVER_CONTROL = false;
        public const int PROCESS_WM_READ = 0x0010;

        static public void Main(string[] args)
        {
            Game game = null;

            //Handling args
            if (args != null && args.Length > 0)
            {
                int i = Array.IndexOf<string>(args, "-names");
                if (i != -1)
                {
                    game = new Game(args[i + 1], args[i + 2]);
                }
            }
            else
            {
                game = new Game();
            }

            while (game.gameMonitor()) { }
            Console.WriteLine("\n\nEnd of Program, you may now close the console.");

            //Prevent console exiting on end to allow users to get the console file.
            while (true)
            {
                Console.Read(); // Pause for testing purposes.
            }
        }


    }
}
