using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace StatsExtractorConsole2
{
    class Program
    {
        const int REFRESH_TIME = 75; //Def 50;

        const float POS_X_LEFT = 3.5f;
        const float POS_X_RIGHT = 16.5f;
        const float POS_Y_BLUE = 4.0f;
        const float POS_Y_RED = 57.0f;

        //Location in memory of variables.
        const int MEMLOC_posX = 0x07D1C290;
        const int MEMLOC_posY = 0x07D1C298;
        const int MEMLOC_posZ = 0x07D1C294;

        const int MEMLOC_vX = 0x07D1C2F4;
        const int MEMLOC_vY = 0x07D1C2D8;

        const int MEMLOC_Time = 0x07D349A8; //Type = Int (4-byte)
        const int MEMLOC_Period = 0x07D349B0; //Type = Int (4-byte)

        const int MEMLOC_RedScore = 0x07D33D98;
        const int MEMLOC_BlueScore = 0x07D33D9C;

        const int MEMLOC_PlayerList = 0x04A5B860;

        const bool SERVER_CONTROL = false;

        const int PROCESS_WM_READ = 0x0010;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
          int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        static public int getTime(IntPtr processHandler)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            ReadProcessMemory((int)processHandler, MEMLOC_Time, buffer, 4, ref bytesRead);
            return System.BitConverter.ToInt32(buffer, 0);
        }

        static public int getTime(MemoryExtractor MemExt)
        {
            return MemExt.getInt32(MEMLOC_Time);
        }

        static public string parseTime(int time)
        {
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

        static public string parseTime(MemoryExtractor MemExt)
        {
            return parseTime(getTime(MemExt));
        }

        static public int getPeriod(MemoryExtractor MemExt)
        {
            int period = MemExt.getInt32(MEMLOC_Period);
            if (getTime(MemExt) == 0)
            {
                period -= 1;
            }
            return period;
        }

        static public int[] getScore(MemoryExtractor MemExt)
            //getScore(processHandler)[0] returns score for red and (...)[1] returns score for blue.
        {
            int[] retVal = new int[2];
            retVal[0] = MemExt.getInt32(MEMLOC_RedScore);
            retVal[1] = MemExt.getInt32(MEMLOC_BlueScore);
            return retVal;
        }

        static public bool gameTied(MemoryExtractor MemExt)
        {
            int[] score = getScore(MemExt);
            return (score[0] == score[1]);
        }

        static List<Player> getPlayers(ref int at_i, MemoryExtractor MemExt)
        {
            List<Player> AddedPlayers = new List<Player>();
            for (int i = at_i; ; i += 152)
            {
                Player newplayer = new Player(i, MemExt);
                if (newplayer.name == "")
                {
                    at_i = i;
                    return AddedPlayers;
                }
                AddedPlayers.Add(newplayer);
            }
        }

        static public void dumpStats(List<Player> hasplayed, Dictionary<string, int> plusminus, Writer file)
        {
            file.WriteLine("\n\n ------ Player stats ------");
            file.WriteLine("\n\nRED TEAM\n");
            foreach (Player player in hasplayed)
            {
                if (player.team_played[0])
                {
                    file.WriteLine(player.name + "   " + player.getStats() + "  +/- = " + plusminus[player.name]);
                }
            }
            file.WriteLine("\n\nBLUE TEAM\n");
            foreach (Player player in hasplayed)
            {
                if (player.team_played[1])
                {
                    file.WriteLine(player.name + "   " + player.getStats() + "  +/- = " + plusminus[player.name]);
                }
            }
        }


        static void Main(string[] args)
        {
            MemoryExtractor MemExt = new MemoryExtractor();
            Writer file = new Writer();

            Dictionary<string,int> plusminus = new Dictionary<string, int>();

            List<Player> Allplayers = new List<Player>();
            List<Player> redPlayers = new List<Player>();
            List<Player> bluePlayers = new List<Player>();

            int[] score = getScore(MemExt);

            int at_i = MEMLOC_PlayerList;
            //Initial assignment
            foreach (Player player in getPlayers(ref at_i, MemExt))
            {
                Allplayers.Add(player);
                if (!plusminus.ContainsKey(player.name))
                {
                    plusminus.Add(player.name, 0);
                }
            }

            bool timeStopped = false;

            Console.WriteLine("Current Players Loaded");
            bool endperiod = false;
            int teamScored = -1;

            Console.WriteLine("Entered at " + parseTime(getTime(MemExt)) + " of period " + getPeriod(MemExt).ToString());
            while (true)
            {
                foreach (Player newplayer in getPlayers(ref at_i, MemExt))
                {
                    Allplayers.Add(newplayer);
                    if (!plusminus.ContainsKey(newplayer.name))
                    {
                        plusminus.Add(newplayer.name, 0);
                    }
                    Console.WriteLine(newplayer.name + " has joined the server!");
                }
                
                System.Threading.Thread.Sleep(REFRESH_TIME);
                int time1 = getTime(MemExt);
                System.Threading.Thread.Sleep(75);
                int time2 = getTime(MemExt);
                foreach (Player player in Allplayers)
                {
                    player.checkOnIce();
                }
                if (time2 == 0)
                {
                    if (!endperiod) //Whether the messages have already been submitted
                    {
                        int[] current_score = getScore(MemExt);
                        int period = getPeriod(MemExt);
                        if (period == 0)
                        {
                            Console.WriteLine("Game Starting!");
                        }
                        else if (period >= 3 && current_score[0] != current_score[1])
                        {
                            if (current_score[0] > current_score[1])
                            //Red wins
                            {
                                file.WriteLine("Game Over, Red Team wins " + current_score[0].ToString()
                                    + "-" + current_score[1].ToString());
                            }
                            else
                            {
                                file.WriteLine("Game Over, Blue Team wins " + current_score[0].ToString()
                                    + "-" + current_score[1].ToString());
                            }
                            dumpStats(Allplayers, plusminus, file);
                        }
                        else
                        {
                            file.WriteLine("");
                            Console.WriteLine("End of period " + getPeriod(MemExt).ToString() + ".");
                            file.WriteLine(" ---- Period " + (getPeriod(MemExt) + 1).ToString() + " Start ----");
                        }
                    }
                    endperiod = true;
                }
                else
                {
                    endperiod = false;
                }
                if (time1 == time2 && time1 != 0)
                {
                    if (timeStopped)
                    {
                        continue;
                    }

                    if (score == getScore(MemExt))
                    {
                        Console.WriteLine("Problem Detected!");
                    }
                    else if (score[0] < getScore(MemExt)[0])
                    {
                        Console.WriteLine("REDGOL!");
                        teamScored = 0;
                    }
                    else
                    {
                        Console.WriteLine("BLUEGOL");
                        teamScored = 1;
                    }

                    score = getScore(MemExt);

                    timeStopped = true;
                    Player goalscorer = null;
                    bool goal;
                    bool assist;
                    Player assister = null;

                    List<string> OnIceNames = new List<string>();
                    List<Player> OnIce = new List<Player>();

                    foreach (Player player in Allplayers)
                    {
                        goal = false;
                        assist = false;

                        if (player.checkOnIce())
                        {
                            OnIce.Add(player);
                            OnIceNames.Add(player.name);
                            if (player.CheckPoints(ref goal, ref assist))
                            {
                                if (goal)
                                {
                                    goalscorer = player;
                                }
                                else if (assist)
                                {
                                    assister = player;
                                }
                            }
                        }
                    }

                    if (goalscorer != null)
                    {
                        string toWrite = getScore(MemExt)[0].ToString() + "-" + getScore(MemExt)[1].ToString();
                        Console.WriteLine("["+parseTime(MemExt)+"] GOL!");
                        Console.WriteLine("Scored by: " + goalscorer.name + " " + goalscorer.getStats());
                        toWrite += " [" + parseTime(MemExt) + "] - " + goalscorer.name + " " + goalscorer.getStats();

                        if (assister != null)
                        {
                            Console.WriteLine("Assisted by: " + assister.name + " " + assister.getStats());
                            toWrite += " -- Assisted by: " + assister.name + " " + assister.getStats();
                        }

                        file.WriteLine(toWrite);
                        Console.WriteLine("Players On Ice : ");

                        foreach (string name in OnIceNames)
                        {
                            Console.WriteLine(" * " + name);
                        }

                        foreach (Player player in OnIce)
                        {
                            if (player.team == teamScored)
                            {
                                if (plusminus.ContainsKey(player.name))
                                {
                                        plusminus[player.name] += 1;
                                    
                                }
                                else
                                {
                                    Console.WriteLine("Error WTF player " + player.name);
                                }
                            }
                            else
                            {
                                plusminus[player.name] -= 1;
                            }
                        }
                    }
                }
                else
                {
                    timeStopped = false;
                }
                teamScored = -1;
            }

        }
    }
}