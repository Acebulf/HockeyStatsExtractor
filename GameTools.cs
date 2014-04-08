using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsExtractorConsole2
{
    abstract class GameTools
    {
        public MemoryExtractor MemExt;
        public Writer writer;

        public int getScore(int i)
        {
            return getScore()[i];
        }

        public int[] getScore()
        //getScore()[0] returns score for red and (...)[1] returns score for blue.
        {
            int[] retVal = new int[2];
            retVal[0] = MemExt.getInt32(MEMLOC.RedScore);
            retVal[1] = MemExt.getInt32(MEMLOC.BlueScore);
            
            //Prevent weird things from happening.
            if (retVal[0] >= 0 && retVal[1] >= 0)
            {
                return retVal;
            }
            else
            {
                throw new System.IO.InvalidDataException("Scores are somehow negative. Has the game been exited?");
            }
        }

        public int getTime()
        //Returns the time left in period in seconds.
        {
            return MemExt.getInt32(MEMLOC.Time);
        }

        public string parseTime(int time)
        //Returns a string from the number of seconds in the format
        //(minutes) : seconds
        {
            int seconds = time / 100;
            int minutes = seconds / 60;
            string second;
            if (seconds % 60 < 10)
            {
                second = "0" + (seconds % 60).ToString();
            }
            else
            {
                second = (seconds % 60).ToString();
            }
            return (minutes).ToString() + ":" + second;
        }

        public string parseTime()
        {
            return parseTime(getTime());
        }

        public int getPeriod()
        {
            int period = MemExt.getInt32(MEMLOC.Period);

            //The period is incremented as soon as the period ends, which is not how period are usually defined.
            if (getTime() == 0)
            {
                period -= 1;
            }
            return period;
        }

        public void writePeriod()
        {
            int period = getPeriod() - 1;
            writer.WriteLine("\n\n-------------- PERIOD " + period + " STARTING --------------------");
            Console.WriteLine("Period " + period + " starting");
        }

        public bool timeIsStopped()
        {
            if (MemExt.getInt32(MEMLOC.StopTime) == 0)
                return false;
            return true;
        }

        public bool gameIsOver()
        {
            if (MemExt.getInt32(MEMLOC.GameOver) == 1)
                return true;
            return false;
        }

        protected List<PlayerHolder> getPlayers(ref int at_i)
        {
            List<PlayerHolder> AddedPlayers = new List<PlayerHolder>();
            for (int i = at_i; ; i += 152)
            {
                PlayerHolder newplayer = new PlayerHolder(i, MemExt);
                if (newplayer.name == "")
                {
                    at_i = i;
                    return AddedPlayers;
                }
                AddedPlayers.Add(newplayer);
            }
        }
    }
}
