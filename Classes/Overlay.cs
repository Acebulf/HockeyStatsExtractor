using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsExtractorConsole2
{
    class Overlay
    {
        public string redScorePath;
        public string redNamePath;
        public string blueScorePath;
        public string blueNamePath;

        public Writer redWriter;
        public Writer blueWriter;

        public Overlay(string _redScorePath, string _redNamePath, string _blueScorePath, string _blueNamePath)
        {
            blueScorePath = _blueScorePath;
            blueNamePath = _blueNamePath;
            redScorePath = _redScorePath;
            redNamePath = _redNamePath;
            redWriter = new Writer(redScorePath);
            blueWriter = new Writer(blueScorePath);
        }

        public Overlay(string _redScorePath, string _blueScorePath)
        {
            redScorePath = _redScorePath;
            blueScorePath = _blueScorePath;
            redWriter = new Writer(redScorePath);
            blueWriter = new Writer(blueScorePath);
        }

        public void updateScore(int redScore, int blueScore)
        {
            redWriter.WriteFile(redScore.ToString());
            blueWriter.WriteFile(blueScore.ToString());
        }
    }
}
