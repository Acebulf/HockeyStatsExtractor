using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StatsExtractorConsole2
{

    class Writer
    {
        bool consolePrint = false;
        public string getTime()
            //Returns digits for filename, but maintains the files' chronological order.
        {
            DateTime Jan1_1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ((long)((DateTime.UtcNow - Jan1_1970).TotalMilliseconds) / 1000).ToString();
        }

        void createFilePath()
        {
            string path = Directory.GetCurrentDirectory();

            if (!Directory.Exists(path+ @"\stats\"))
            {
                Directory.CreateDirectory(path + @"\stats\");
            }
            string add2path = getTime();
            FilePath = path + @"\stats\" + add2path + ".hstats";
        }

        string FilePath;
        public Writer(string Path)
        {
            FilePath = Path;
        }

        public Writer()
        {
            createFilePath();
        }

        public Writer(bool ConsolePrint)
        {
            createFilePath();
            consolePrint = ConsolePrint;
        }

        public void WriteLine(string text)
        {
            //Defaults to consoleprint
            WriteLine(text, consolePrint);
        }

        public void WriteLine(string text, bool consoleWrite)
        {
            using (StreamWriter file = new StreamWriter(FilePath, true))
            {
                file.WriteLine(text);
            }

            if (consoleWrite)
            {
                Console.WriteLine(text);
            }
        }

        public void WriteFile(string text)
            //Overwrites file
        {
            using (StreamWriter file = new StreamWriter(FilePath, false))
            {
                file.WriteLine(text);
            }
        }
    }
}
