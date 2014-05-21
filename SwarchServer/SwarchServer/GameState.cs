using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace SwarchServer
{
    class GameState
    {
        public const float PLAYER_ONE_START_POSITION_X = -3.0f;
        public const float PLAYER_ONE_START_POSITION_Y = 3.0f;
        public const float PLAYER_TWO_START_POSITION_X = 3.0f;
        public const float PLAYER_TWO_START_POSITION_Y = -3.0f;
        public string roomName;
        public int roomID;
        public int pelletNumber;
        public bool gameStarted = false;
        public ArrayList playerList;
        public Pellet[] pelletList;
        public bool isServerRunning = true;
        //public SQLiteDB db = new SQLiteDB();
        Thread gameLoop;

        public GameState(string name, int id)
        {
            roomName = name;
            roomID = id;
            pelletNumber = 0;
            playerList = new ArrayList();
            pelletList = new Pellet[8];

            gameLoop = new Thread(new ThreadStart(serverLoop));
            gameLoop.Start();
            //db.dbConnect("users.sqlite");
        }

        void Update()
        {
            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }

            foreach (Player player in lockedPlayerList)
            {
                //grab the readqueue then unlock the readqueue.

                lock (player.readQueueGameState)
                {
                    Queue<Command> prq = player.readQueueGameState;

                    while (prq.Count != 0)
                    {
                        Command cmd = prq.Dequeue();
                        switch (cmd.cType)
                        {
                            case CType.PlayerPosition:
                                playerPositionResponse(player, cmd.x, cmd.y, cmd.dir);
                                break;
                            case CType.SizeUpdate:
                                break;
                            case CType.EatPellet:
                                break;
                            case CType.EatPlayer:
                                break;
                            case CType.Death:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            for (int i = 0; i < lockedPlayerList.Length; ++i)
            {
                for (int n = i + 1; n < lockedPlayerList.Length; ++n)
                {
                    Player p1 = lockedPlayerList[i];
                    Player p2 = lockedPlayerList[n];
                    if (!p1.isDead && !p2.isDead)
                    {
                        Rectangle r = Rectangle.Intersect(p1.playerRect, p2.playerRect);
                        if (!r.IsEmpty)
                        {
                            if (p1.size > p2.size)
                            {
                                eatPlayer(p1, p2);
                            }
                            else if (p1.size < p2.size)
                            {
                                eatPlayer(p2, p1);
                            }
                            else
                            {
                                death(p1);
                                death(p2);
                            }
                        }
                    }
                }
            }
        }

        public void startGame()
        {
            for (int i = 0; i < pelletList.Length; ++i)
            {
                spawnPellet(i);
            }
            
            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }
            int dir = 1;
            foreach (Player player in lockedPlayerList)
            {
                if (dir > 3) break;
                player.setPosition(dir, (dir==1?PLAYER_ONE_START_POSITION_X:PLAYER_TWO_START_POSITION_X),(dir==1?PLAYER_ONE_START_POSITION_Y:PLAYER_TWO_START_POSITION_Y));
                player.sendCommand(Command.startGameCommand(0, lockedPlayerList[0], lockedPlayerList[1], PLAYER_ONE_START_POSITION_X, PLAYER_ONE_START_POSITION_Y, PLAYER_TWO_START_POSITION_X, PLAYER_TWO_START_POSITION_Y, 3, 1, pelletList));
                dir += 2;
            }
            gameStarted = true;
        }

        public void playerPositionResponse(Player p, float x, float y, int dir)
        {
            p.setPosition(dir, x, y);
            
            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }
            foreach(Player player in lockedPlayerList)
            {
                if(player.playerNumber != p.playerNumber)
                {
                    player.sendCommand(Command.playerPositionCommand(0, p, x, y, dir));
                }
            }
        }

        public void spawnPellet(int index)
        {
            pelletList[index] = new Pellet(pelletNumber);
            pelletNumber += 1;
        }

        public void eatPellet(int id, Player p)
        {
            for(int i = 0; i < pelletList.Length; ++i)
            {
                if(id == pelletList[i].id)
                {
                    p.increaseSize(pelletList[i].size);
                    Console.WriteLine("Size: " + p.size);
                    int oldPelletID = pelletList[i].id;
                    spawnPellet(i);
                    Player[] lockedPlayerList;

                    lock (playerList)
                    {
                        lockedPlayerList = new Player[playerList.Count];
                        playerList.CopyTo(lockedPlayerList);
                    }
                    foreach (Player player in lockedPlayerList)
                    {
                        player.sendCommand(Command.eatPelletCommand(0, p, oldPelletID, pelletList[i]));
                    }
                    break;
                }
            }
        }

        public void eatPlayer(Player p1, Player p2)
        {
            p2.isDead = true;
            p1.increaseSize(p2.size);

            p2.resetPosition();

            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }
            foreach (Player player in lockedPlayerList)
            {
                player.sendCommand(Command.eatPlayerCommand(0, p1, p2));
            }
            //p2.isDead = false;
        }

        public void death(Player p)
        {
            p.isDead = true;
            p.resetPosition();
            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }
            foreach (Player player in lockedPlayerList)
            {
                player.sendCommand(Command.deathCommand(0, p));
            }
            //p.isDead = false;
        }

        public void sendPlayerInfo(Player player)
        {
            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }

            for (int i = 0; i < lockedPlayerList.Length; i++)
            {
                lockedPlayerList[i].sendCommand(Command.newPlayerCommand(0, player.playerName, player.playerNumber));

                if (player != lockedPlayerList[i])
                {
                    player.sendCommand(Command.newPlayerCommand(0, ((Player)lockedPlayerList[i]).playerName, ((Player)lockedPlayerList[i]).playerNumber));
                }
            }
        }

        public void sendPlayerInfoOnLeave(Player player)
        {
            Player[] lockedPlayerList;

            lock (playerList)
            {
                lockedPlayerList = new Player[playerList.Count];
                playerList.CopyTo(lockedPlayerList);
            }

            for (int i = 0; i < lockedPlayerList.Length; i++)
            {
                lockedPlayerList[i].sendCommand(Command.leftPlayerCommand(0, player.playerName, player.playerNumber));
            }
        }
        
        public void addPlayer(Player p)
        {
            lock (playerList)
            {
                playerList.Add(p);
            }
            sendPlayerInfo(p);
        }

        public void removePlayer(Player p)
        {
            lock (playerList)
            {
                playerList.Remove(p);
            }
            sendPlayerInfoOnLeave(p);
        }

        public int numberOfPlayers()
        {
            return playerList.Count;
        }

        public void stopServer()
        {
            isServerRunning = false;
            gameLoop.Abort();
        }

        private void serverLoop()
        {
            while(isServerRunning)
            {
                Update();
            }
        }
    }
}
