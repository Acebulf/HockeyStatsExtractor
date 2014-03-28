using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StatsExtractorConsole2
{
    //The game class is derived from the GameTools class which contains tools useful for the Game class.

    class Game : GameTools
    {
        string nameRed;
        string nameBlue;
        bool exitOnFinish = true;
        int scoreRed;
        int scoreBlue;
        int period;
        //bool periodEnd = false;
        bool timeStoppedHandled = false;

        int at_i = MEMLOC.PlayerList;

        List<PlayerHolder> playerHolderList;

        public void seekNewPlayerHolders()
            //Adds new PlayerHolders to the playerHolderList.
        {
            foreach (PlayerHolder newPlayerHolder in getPlayers(ref at_i))
            {
                playerHolderList.Add(newPlayerHolder);
            }
        }

        public void checkPlayerHolderIdentities()
        {
            foreach (PlayerHolder pH in playerHolderList)
            {
                pH.checkIdentity();
            }
        }

        public void checkOnIce()
        {
            foreach (PlayerHolder pH in playerHolderList)
            {
                pH.checkOnIce();
            }
        }

        public void gameInit()
            //All the operations that are executed at the initialization that are common
            //for all overloaded constructors.
        {
            Player.reinitialize(); //Make sure that the Player class' static variables are reset.

            MemExt = new MemoryExtractor();
            writer = new Writer();

            if (getPeriod() > 0)
            {
                writer.WriteLine("WARNING: GAME STARTED BEFORE THE PROGRAM.");
                if (getScore(0) + getScore(1) != 0)
                {
                    writer.WriteLine("CRITICAL WARNING: GOALS HAVE BEEN SCORED BEFORE THE PROGRAM WAS RUNNING.");
                    writer.WriteLine("STATS ARE NOT VALID. MUST BE AMENDED MANUALLY.");
                }

                writer.WriteLine("\n");
            }

            playerHolderList = new List<PlayerHolder>();
            updateScore();
            period = getPeriod();

            seekNewPlayerHolders();

            Console.WriteLine("Entered at " + parseTime() + " of period " + getPeriod().ToString());
        }

        public void gameOver()
        {
            int winScore;
            int loseScore;
            string winner = nameBlue;
            if (getScore(0) > getScore(1))
            {
                winner = nameRed;
                winScore = getScore(0);
                loseScore = getScore(1);
            }
            else
            {
                winner = nameBlue;
                loseScore = getScore(0);
                winScore = getScore(1);
            }
            writer.WriteLine("\n\n Game Over! " + winner + " wins " + winScore + " - " + loseScore + "\n\n", true);
            
            dumpStats();
        }

        public void dumpStats()
        {
            if (Player.PlayerDict != null)
            {
                foreach (string name in Player.PlayerDict.Keys)
                {
                    Console.WriteLine(name);
                }

                writer.WriteLine("\n\n ------ Player stats ------");
                writer.WriteLine("\n" + nameRed + "\n\n");


                foreach (Player player in Player.PlayerDict.Values)
                {
                    if (player.playedTeam[0])
                    {
                        writer.WriteLine(player.PlayerStats());
                    }
                }
                writer.WriteLine("\n\n\n" + nameBlue +"\n\n");

                foreach (Player player in Player.PlayerDict.Values)
                {
                    if (player.playedTeam[1])
                    {
                        writer.WriteLine(player.PlayerStats());
                    }
                }
                Console.WriteLine("PrintedSwage");
            }
        }

        public bool gameMonitor()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            while (true)
            {
                seekNewPlayerHolders();
                checkPlayerHolderIdentities();

                //Only proceed when enough time has passed.
                if (timer.ElapsedMilliseconds <= Program.REFRESH_TIME)
                {
                    continue;
                }

                //Debug step

                if (timer.ElapsedMilliseconds >= 100)
                {
                    Console.WriteLine("Long interval: " + timer.ElapsedMilliseconds.ToString());
                }

                timer.Restart();
                checkOnIce();

                if (getTime() == 0 && getPeriod() >= 3 && getScore(0) != getScore(1))
                {
                    gameOver();
                    return false;
                }


                if (timeIsStopped() && !timeStoppedHandled)
                {
                    int teamScored = -1;
                    if (getPeriod() == 0)
                    {
                        writer.WriteLine("Game Starting: " + nameRed + " vs. " + nameBlue, true);
                        timeStoppedHandled = true;
                    }

                    else if (getTime() == 0)
                    //Period End
                    {
                        if (getPeriod() >= 3 && getScore(0) == getScore(1))
                        //Overtime
                        {
                            writePeriod();
                            Console.WriteLine("Game going into overtime!");
                        }
                        else if (getPeriod() >= 3)
                        {
                            gameOver();
                            return false;
                        }
                        else
                        {
                            writePeriod();
                        }
                        timeStoppedHandled = true;
                    }

                    else
                        //Goal Scored
                    {
                        Console.WriteLine("GOL!");
                        if (scoreRed < getScore(0))
                        //Red goal
                        {
                            teamScored = 0;
                        }
                        else if (scoreBlue < getScore(1))
                        //Blue goal
                        {
                            teamScored = 1;
                        }
                        updateScore();
                        timeStoppedHandled = true;

                        PlayerHolder goalscorer = null;
                        PlayerHolder assister = null;

                        List<string> OnIceNames = new List<string>();
                        List<PlayerHolder> OnIcePH = new List<PlayerHolder>();

                        foreach (PlayerHolder playerHolder in playerHolderList)
                        {
                            bool goal = false;
                            bool assist = false;

                            if (playerHolder.checkOnIce())
                            {
                                OnIcePH.Add(playerHolder);
                                OnIceNames.Add(playerHolder.name);

                                if (playerHolder.CheckPoints(ref goal, ref assist))
                                {
                                    if (goal)
                                    {
                                        goalscorer = playerHolder;
                                    }
                                    if (assist)
                                    {
                                        assister = playerHolder;
                                    }
                                }
                            }
                        }

                        if (goalscorer != null)
                        {
                            string toWrite = getScore(0).ToString() + "-" + getScore(1).ToString();
                            Console.WriteLine("[" + parseTime() + "] GOL");
                            Console.WriteLine("Scored by: " + goalscorer.name + " " + goalscorer.getStats());
                            toWrite += " [" + parseTime() + "] - " + goalscorer.name + " " + goalscorer.getStats();

                            if (assister != null)
                            {
                                Console.WriteLine("Assisted by: " + assister.name + " " + assister.getStats());
                                toWrite += " -- Assisted by: " + assister.name + " " + assister.getStats();
                            }

                            writer.WriteLine(toWrite);
                            Console.WriteLine("Players on ice: ");

                            foreach (string name in OnIceNames)
                            {
                                Console.WriteLine(name);
                            }

                            foreach (PlayerHolder player in OnIcePH)
                            {
                                if (player.team == teamScored)
                                {
                                    Player.PlayerDict[player.name].plusminus += 1;
                                }
                                else if (player.team != teamScored)
                                {
                                    Player.PlayerDict[player.name].plusminus += -1;
                                }
                            }
                        }
                    }


                }

                else if (!timeIsStopped())
                {
                    timeStoppedHandled = false;
                }
            }
        }

        private void updateScore()
        {
            scoreRed = getScore(0);
            scoreBlue = getScore(1);
        }

        public Game(string _nameRed, string _nameBlue)
        {
            nameRed = _nameRed;
            nameBlue = _nameBlue;
            gameInit();
        }

        public Game(string _nameRed, string _nameBlue, bool _exitOnFinish)
        {
            nameRed = _nameRed;
            nameBlue = _nameBlue;
            exitOnFinish = _exitOnFinish;
            gameInit();
        }

        public Game()
        {
            nameRed = "Red Team";
            nameBlue = "Blue Team";
            gameInit();
        }

    }
}
