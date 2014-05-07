using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace SwarchServer
{
    class GameState
    {
        private ArrayList playerList;

        public GameState()
        {
            playerList = new ArrayList();
        }

        void Update()
        {
            foreach(Player player in playerList)
            {
                Queue<Command> prq = player.readQueue;
                lock(prq)
                {
                    while(prq.Count != 0)
                    {
                        Command cmd = prq.Dequeue();
                        switch(cmd.cType)
                        {
                            case CType.Login:
                                loginPlayer(cmd.username, cmd.password);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void loginPlayer(string username, string password)
        {
            Console.WriteLine("Username: " + username);
            Console.WriteLine("Password: " + password);
        }

        public void addPlayer(Player p)
        {
            playerList.Add(p);
        }

        public void removePlayer(Player p)
        {
            playerList.Remove(p);
        }

        public int numberOfPlayers()
        {
            return playerList.Count;
        }
    }
}
