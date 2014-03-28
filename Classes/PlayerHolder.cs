using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace StatsExtractorConsole2
{
    class PlayerHolder
    {
        /**
         *The PlayerHolder class is a class which fetches and parses the memory region reserved for that
         * player in the hockey.exe. It does not correspond only to a single player, as if a player
         * leaves the server, the information remains and is (partially) overwritten. Therefore, statskeeping
         * in this class serves only to create issues.
         * 
         * In version a0.3, the Player class was split into the Player class for the statskeeping of an individual
         * player by name, and the PlayerHolder class which does the parsing of the memory region for which it
         * is associated.
         * **/

        int MEMLocation;
        public MemoryExtractor MemExt;
        public bool inServer;
        public int position;

        private int _TEAM;
        public int team
        {
            get { return _TEAM; }

            set
            {
                _TEAM = value;

                if (team == 0 || team == 1)
                {
                    team_played[team] = true;
                    hasPlayed = true;

                    if (player != null) //Stops crash on first assign
                    {
                        player.assignTeam(team);
                    }
                }
            }
        }

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

        Player player;

        private void checkNewPlayer()
        {
            refresh();
            if (Player.PlayerDict == null || (Player.PlayerDict != null && !Player.PlayerDict.ContainsKey(name)))
            {
                player = new Player(name, goals, assists, 0);
            }
            else
            {
                player = Player.PlayerDict[name];
            }
        }

        public void checkIdentity()
            //Checks if the identity of the Player in the PlayerHolder has changed.
        {
            if (getName() != player.name)
            {
                reset();
            }
        }

        public string getName()
        {
            return MemExt.getString(MEMLocation + MEM_OFFSET_name, NAME_LEN);
        }

        private void reset()
            //Resets the playerHolder to take in a new player. 
        {
            plusminus = 0;
            refresh();
            checkNewPlayer();
        }

        public PlayerHolder(int MEM_Location, MemoryExtractor MemoryExt)
        {
            MEMLocation = MEM_Location;
            MemExt = MemoryExt;

            onIce = false;
            refresh();

            checkNewPlayer();
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

        public void plusMinusChange(int amount)
        {
            player.plusminus += amount;
        }

        public bool checkOnIce()
        {
            inServer = MemExt.getBool(MEMLocation + MEM_OFFSET_inServer);
            if (!inServer)
            {
                if (onIce)
                {
                    Console.WriteLine("[" + parseTime() + "] - Player " + name + " has left the ice.");
                    onIce = false;
                    refresh();
                }
                return false;
            }

            if (MemExt.getInt32(MEMLocation + MEM_OFFSET_team) != -1)
            {
                if (!onIce)
                {
                    refresh();
                    Console.WriteLine("["+ parseTime() + "] - Player " + name + " has entered the ice.");
                }
                onIce = true;
                return true;
            }
            //If the player is not on a team, but is stated to be on the ice (state change)
            else if (onIce)
            {
                Console.WriteLine("[" + parseTime() + "] - Player " + name + " has left the ice.");
                refresh();
            }
            onIce = false;
            return false;
        }

        //Refresh calls are computationally intensive and also causes bugs in areas where a state change is checked.
        private void refresh()
        {
            MemExt.getRefreshData(MEMLocation, 152);
            inServer = MemExt.refreshBool(MEM_OFFSET_inServer);
            position = MemExt.refreshInt32(MEM_OFFSET_position);
            team = MemExt.refreshInt32(MEM_OFFSET_team);
            name = MemExt.refreshString(MEM_OFFSET_name, NAME_LEN);
            goals = MemExt.refreshInt32(MEM_OFFSET_goals);
            assists = MemExt.refreshInt32(MEM_OFFSET_assists);
        }

        public string getStats()
        {
            return " (" + goals.ToString() + "-" + assists.ToString() + ")";
        }

        private string parseTime()
        {
            int time = MemExt.getInt32(MEMLOC.Time);

            int seconds = time / 100;
            int minutes = seconds/60;
            string second;
            if (seconds % 60 < 10)
            {
                second = "0"+(seconds % 60).ToString();
            }
            else
            {
                second = (seconds%60).ToString();
            }
            return (minutes).ToString() + ":" + second;
        }
    }
}
