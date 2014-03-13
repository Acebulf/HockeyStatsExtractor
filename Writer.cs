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
        public string getTime()
        {
            DateTime Jan1_1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ((long)((DateTime.UtcNow - Jan1_1970).TotalMilliseconds) / 1000).ToString();
        }

        string FilePath;
        StreamWriter file;
        public Writer(string Path)
        {
            FilePath = Path;
        }

        public Writer()
        {
            string path = Directory.GetCurrentDirectory();
            string add2path = getTime();
            FilePath = path + add2path + ".hstats";    
        }

        public void WriteLine(string text)
        {
            file = new StreamWriter(FilePath, true);
            file.WriteLine(text);
            file.Close();
        }
    }
}
