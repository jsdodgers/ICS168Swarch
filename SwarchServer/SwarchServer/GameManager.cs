using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SwarchServer
{
    class GameManager
    {
        private static int port = 7589;
        private static PlayerListener listener;
        public static ArrayList playerList = new ArrayList();
        public static int numberOfCurrentPlayers = 0;
        public const int NUMBER_OF_GAMES = 4;
        public static GameState[] gss = null;
        public static SQLiteDB db = new SQLiteDB();
        static Thread gameLoop;
        public static bool isServerRunning = true;
        //public static GameState gs;
        private static bool isQuitted = false;
        private static bool gameStarted = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Multiplayer Swarch Server:");
            //Console.WriteLine("Please enter a command.  For available commands type HELP.");
            db.dbConnect("users.sqlite");
            gameLoop = new Thread(new ThreadStart(serverLoop));
            gameLoop.Start();
            startServer();
            while(true)
            {
                foreach(GameState gameState in gss)
                {
                    if (gameState.playerList.Count > 1 && !gameState.gameStarted)
                    {
                        startGame(gameState);
                    }
                    if(gameState.playerList.Count < 1)
                    {
                        gameState.gameStarted = false;
                    }
                }
            }
            /*
            while(true)
            {
                switch (Console.ReadLine())
                {
                    case "START SERVER":
                        startServer();
                        break;
                    case "START GAME":
                        startGame();
                        break;
                    case "HELP":
                    case "COMMANDS":
                        help();
                        break;
                    case "RESET":
                        reset();
                        break;
                    case "QUIT":
                    case "Q":
                        quit();
                        break;
                    default:
                        Console.WriteLine("Invalid command.  Type HELP for valid commands.");
                        break;
                }
            }*/
        }

        private static void startServer()
        {
            if(gss != null)
            {
                Console.WriteLine("Game is already in progress.  Please reset or quit to start a new game.");
            }
            else
            {
                gss = new GameState[NUMBER_OF_GAMES];

                gameStarted = false;
                for (int i = 0; i < NUMBER_OF_GAMES; i++ )
                {
                    gss[i] = new GameState("Foo foo for you " + i, i);
                }
                
                listener = new PlayerListener();
                Console.WriteLine("The server is up, players can now connect.");
            }
        }

        private static void startGame(GameState gs)
        {
            gameStarted = true;
            gs.startGame();
            Console.WriteLine("A game has started in " + gs.roomName + ".  Good luck and have fun!");
        }

        public class PlayerListener
        {
            private static TcpListener tcpListener;
            private Thread listenerThread;

            public PlayerListener()
            {
                listenerThread = new Thread(new ThreadStart(openSockets));
                listenerThread.Start();
            }

            private void openSockets()
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                
                while(!isQuitted)
                {
                    TcpClient playerClient = tcpListener.AcceptTcpClient();
                    NetworkStream stream = playerClient.GetStream();
                    addPlayer(playerClient, stream);
                }
            }
        }

        private static void addPlayer(TcpClient client, NetworkStream stream)
        {
            //gs.addPlayer(new Player(client, stream, gs.numberOfPlayers(), gs));
            lock (playerList)
            {
                playerList.Add(new Player(client, stream, numberOfCurrentPlayers));
            }
            numberOfCurrentPlayers += 1;
        }

        private static void help()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("    -START SERVER: Boots up the server and lets players connect.");
            Console.WriteLine("    -START GAME: Starts the game with current number of players.");
            Console.WriteLine("    -HELP | COMMANDS: States commands and their functions.");
            Console.WriteLine("    -RESET: Resets the server, disconnecting players and resets the board.");
            Console.WriteLine("    -QUIT | Q: Shuts down the server and ends this program.");
        }

        static void Update()
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
                
                lock (player.readQueue)
                {
                    Queue<Command> prq = player.readQueue;

                    while (prq.Count != 0)
                    {
                        Command cmd = prq.Dequeue();
                        switch (cmd.cType)
                        {
                            case CType.Login:
                                loginPlayer(player, cmd.username, cmd.password);
                                break;
                            case CType.JoinGame:
                                joinGame(cmd.playerRoom, player);
                                break;
                            case CType.LeaveGame:
                                leaveGame(cmd.playerRoom, player);
                                break;
                            case CType.Disconnect:
                                player.disconnect();
                                Console.WriteLine(player.playerName + " has disconnected.");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static void loginPlayer(Player player, string username, string password)
        {
            LoginResponseType lrt = LoginResponseType.FailedLogin;
            if (username != "" && password != "")
            {
                string realPassword = db.getPassword(username);
                if (realPassword == null)
                {
                    realPassword = password;
                    db.addUser(username, password);
                    lrt = LoginResponseType.NewUser;
                    Console.WriteLine("New user created.");
                }
                if (realPassword == password)
                {
                    //db.dbPrintAll();
                    lrt |= LoginResponseType.SucceededLogin;
                    player.playerName = username;
                    Console.WriteLine(player.playerName + " has connected.");
                }
            }

            player.sendCommand(Command.loginCommand(0, lrt, gss));
        }

        public static void joinGame(int roomName, Player player)
        {
            player.sendCommand(Command.joinGameCommand(0, roomName));
            gss[roomName].addPlayer(player);
            player.gs = gss[roomName];
            if(gss[roomName].gameStarted)
            {
                Player[] lockedPlayerList;

                lock (playerList)
                {
                    lockedPlayerList = new Player[gss[roomName].playerList.Count];
                    gss[roomName].playerList.CopyTo(lockedPlayerList);
                }
                
                player.resetPosition();
                player.sendCommand(Command.startGameInProgressCommand(0, lockedPlayerList, gss[roomName].pelletList));
            }

            roomUpdateToAllPlayers(player.gs);

            Console.WriteLine(player.playerName + " has joined " + gss[roomName].roomName + ".");
        }

        public static void leaveGame(int roomName, Player player)
        {
            gss[roomName].removePlayer(player);
            roomUpdateToAllPlayers(player.gs);
            player.gs = null;
            player.sendCommand(Command.leaveGameCommand(0, roomName));
            Console.WriteLine(player.playerName + " has left " + gss[roomName].roomName + ".");
        }

        public static void roomUpdateToAllPlayers(GameState roomUpdated)
        {
            foreach (Player p in playerList)
            {
                p.sendCommand(Command.roomUpdateCommand(0, roomUpdated));
            }
        }

        public static void removePlayer(Player p)
        {
            lock (playerList)
            {
                playerList.Remove(p);
            }
        }

        public static void stopServer()
        {
            isServerRunning = false;
            gameLoop.Abort();
        }

        private static void serverLoop()
        {
            while (isServerRunning)
            {
                Update();
            }
        }

        /*private static void reset(GameState gs)
        {
            if(gs != null)
            {
                gameStarted = false;
                gs.stopServer();
                gs = null;
                listener = null;

                Console.WriteLine("Server has been reset.");
            }
            else
            {
                Console.WriteLine("No game exists, yet. Dummy.");
            }
            
        }*/

        private static void quit()
        {
            Console.WriteLine("Server is shutting down.  Goodbye!");
            isQuitted = true;
            Environment.Exit(0);
        }
    }
}
