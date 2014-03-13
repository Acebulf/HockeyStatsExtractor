using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace StatsExtractorConsole2
{
    class Player
    {
        int MEMLocation;
        public MemoryExtractor MemExt;
        public bool inServer;
        public int position;
        public int team;
        public bool[] team_played = { false, false };
        public string name;
        public int goals;
        public int assists;
        public bool onIce;
        public bool hasPlayed = false;
        public int plusminus = 0;

        const int MEM_OFFSET_inServer = 0;
        const int MEM_OFFSET_position = 4;
        const int MEM_OFFSET_team = 8;
        const int MEM_OFFSET_name = 20;
        const int NAME_LEN = 24;
        const int MEM_OFFSET_goals = 136;
        const int MEM_OFFSET_assists = 140;
        const int MEM_OFFSET_LEN = 152;
        
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        public Player(int MEM_Location, MemoryExtractor MemoryExt)
        {
            MEMLocation = MEM_Location;
            MemExt = MemoryExt;

            refresh();

            if (team != -1)
            {
                onIce = true;
                hasPlayed = true;
            }
            else
            {
                onIce = false;
            }
        }

        public int getTeam()
        {
            refresh();
            if (team == 0 || team == 1)
            {
                team_played[team] = true;
            }
            return team;
        }


        public bool CheckPoints(ref bool goal, ref bool assist)
        {
            goal = false;
            assist = false;

            if (MemExt.getInt32(MEMLocation + MEM_OFFSET_goals) > goals)
            {
                goal = true;
                refresh();
                return true;
            }
            if (MemExt.getInt32(MEMLocation + MEM_OFFSET_assists) > assists)
            {
                assist = true;
                refresh();
                return true;
            }
            return false;
        }

        public bool checkOnIce()
        {
            inServer = MemExt.getBool(MEMLocation + MEM_OFFSET_inServer);
            if (!inServer)
            {
                onIce = false;
                refresh();
                return false;
            }

            if (MemExt.getInt32(MEMLocation + MEM_OFFSET_team) != -1)
            {
                if (!onIce)
                {
                    refresh();
                    Console.WriteLine("["+ Program.parseTime(MemExt) + "] - Player " + name + " has entered the ice.");
                }
                onIce = true;
                return true;
            }
            else if (onIce)
            {
                Console.WriteLine("Player " + name + " has left the ice.");
            }
            onIce = false;
            return false;
        }

        public void refresh()
        {
            MemExt.getRefreshData(MEMLocation, 152);
            inServer = MemExt.refreshBool(MEM_OFFSET_inServer);
            position = MemExt.refreshInt32(MEM_OFFSET_position);
            team = MemExt.refreshInt32(MEM_OFFSET_team);
            name = MemExt.refreshString(MEM_OFFSET_name, NAME_LEN);
            goals = MemExt.refreshInt32(MEM_OFFSET_goals);
            assists = MemExt.refreshInt32(MEM_OFFSET_assists);

            if (team == 0 || team == 1)
            {
                team_played[team] = true;
            }

            hasPlayed = true;
        }

        public string getStats()
        {
            return " (" + goals.ToString() + "-" + assists.ToString() + ")";
        }
    }
}
