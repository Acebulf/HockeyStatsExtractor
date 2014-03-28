using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsExtractorConsole2
{
    class Player
    {
        static public Dictionary<string, Player> PlayerDict = null; //Holds all instances of players.
        bool duplicate;

        public string name;
        public int goals;
        public int assists;
        public int plusminus;
        public int team;
        public bool[] playedTeam = new bool[2]{false,false};

        static public void reinitialize()
            //Resets the statics for a new game.
        {
            PlayerDict = null;
        }

        public void assignTeam(int _team)
        {
            team = _team;
            playedTeam[team] = true;
        }

        static public bool addPlayerToList(Player player)
        {
            if (PlayerDict == null)
            {
                PlayerDict = new Dictionary<string, Player>();
            }

            if (!PlayerDict.ContainsKey(player.name))
            {
                PlayerDict.Add(player.name, player);
                return false;
            }
            else
            {
                return true;
            }
        }

        public Player(string _name)
        {
            name = _name;
            goals = 0;
            assists = 0;
            plusminus = 0;
            duplicate = addPlayerToList(this);
        }

        public Player(string _name, int _goals, int _assists, int _plusminus)
        {
            name = _name;
            goals = _goals;
            assists = _assists;
            plusminus = _plusminus;
            duplicate = addPlayerToList(this);
        }

        public string PlayerStats()
        {
            return name + "  " + goals.ToString() + "  " + assists.ToString() + "  " + plusminus.ToString();
        }
    }
}
