using System;
using System.Collections.Generic;
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
        public static int numberOfCurrentPlayers = 0;
        public static GameState gs;
        private static bool gameStarted = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Multiplayer Swarch Server:");
            Console.WriteLine("Please enter a command.  For available commands type HELP.");
     //       SQLiteDatabase db = new SQLiteDatabase();
       //     Dictionary<String, String> insertData = new Dictionary<String, String>();
       //     insertData.Add("USERNAME", "1");
       //     insertData.Add("PASSWORD", "2");
       //     db.Insert("USERS", insertData);
            
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
            }
        }

        private static void startServer()
        {
            if(gs != null)
            {
                Console.WriteLine("Game is already in progress.  Please reset or quit to start a new game.");
            }
            else
            {
                gameStarted = false;
                gs = new GameState();
                listener = new PlayerListener();
                Console.WriteLine("Server is up, players can now connect.  Enter START GAME to start the game.");
            }
        }

        private static void startGame()
        {
            if(gs == null)
            {
                Console.WriteLine("The server has not been setup.  Enter START SERVER to start the server.");
            }
            else
            {
                gameStarted = true;
            }
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
                
                while(!gameStarted)
                {
                    TcpClient playerClient = tcpListener.AcceptTcpClient();
                    NetworkStream stream = playerClient.GetStream();
                    addPlayer(playerClient, stream);
                }
            }
        }

        private static void addPlayer(TcpClient client, NetworkStream stream)
        {
            Console.WriteLine("Player has connected.");
            gs.addPlayer(new Player(client, stream, gs.numberOfPlayers(), gs));
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

        private static void reset()
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
            
        }

        private static void quit()
        {
            Console.WriteLine("Server is shutting down.  Goodbye!");
            Environment.Exit(0);
        }
    }
}
