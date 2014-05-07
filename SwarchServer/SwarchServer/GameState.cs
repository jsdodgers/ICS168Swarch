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

        public void addPlayer(Player p)
        {
            playerList.Add(p);
        }

        public int numberOfPlayers()
        {
            return playerList.Count;
        }
    }
}
