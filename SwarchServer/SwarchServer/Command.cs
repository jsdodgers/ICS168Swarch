using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarchServer
{
    public enum CType : byte {Login, StartGame, JoinGame, LeaveGame, NewPlayer, LeftPlayer, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect, RoomUpdate}
    public enum LoginResponseType : int {FailedLogin = 0, SucceededLogin = 1 << 0, NewUser = 1 << 1}

    class Command
    {
        public string message, username, password;
        public float x, y;
        public int dir;
        public int playerRoom;
        public CType cType;
        private long timeStamp;
        private int playerNumber;
        public int[] scores;
        private const char delimiter = ':';
        public LoginResponseType loginResponse;

        public static Command loginCommand(long ts, LoginResponseType t, GameState[] gss)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.Login;
            newCommand.loginResponse = t;
            newCommand.message = newCommand.cType + ":" + t;
            newCommand.message += ":" + gss.Length;
            if((newCommand.loginResponse & LoginResponseType.SucceededLogin) != 0)
            {
                foreach (GameState gs in gss)
                {
                    newCommand.message += ":" + gs.roomName + ":" + gs.numberOfPlayers();
                }
            }

            newCommand.message += ";";

            return newCommand;
        }

        public static Command startGameCommand(long ts, Player p1, Player p2, float x1, float y1, float x2, float y2, int d1, int d2, Pellet[] pelletList)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.StartGame;
            newCommand.message = newCommand.cType + ":" + 0 + ":" + p1.playerNumber + ":" + x1 + ":" + y1 + ":" + d1 + ":" + p2.playerNumber + ":" + x2 + ":" + y2 + ":" + d2;
            for (int i = 0; i < pelletList.Length; ++i)
            {
                newCommand.message += ":" + pelletList[i].id + ":" + pelletList[i].x + ":" + pelletList[i].y + ":" + pelletList[i].size;
            }

            newCommand.message += ";";

            return newCommand;
        }

        public static Command startGameInProgressCommand(long ts, Player[] playerList, Pellet[] pelletList)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.StartGame;
            newCommand.message = newCommand.cType + ":" + 1 + ":" + playerList.Length;

            for (int i = 0; i < playerList.Length; ++i )
            {
                newCommand.message += ":" + playerList[i].playerNumber + ":" + playerList[i].x + ":" + playerList[i].y + ":" + playerList[i].dir + ":" + playerList[i].size;
            }
            newCommand.message += ":" + pelletList.Length;

            for (int i = 0; i < pelletList.Length; ++i)
            {
                newCommand.message += ":" + pelletList[i].id + ":" + pelletList[i].x + ":" + pelletList[i].y + ":" + pelletList[i].size;
            }

            newCommand.message += ";";

            return newCommand;
        }

        public static Command newPlayerCommand(long ts, string username, int n)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.NewPlayer;
            newCommand.username = username;
            newCommand.playerNumber = n;
            newCommand.message = newCommand.cType + ":" + newCommand.username + ":" + newCommand.playerNumber + ";";
            return newCommand;
        }

        public static Command joinGameCommand(long ts, int room)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.JoinGame;
            newCommand.playerRoom = room;
            newCommand.message = newCommand.cType + ":" + newCommand.playerRoom + ";";
            return newCommand;
        }

        public static Command leaveGameCommand(long ts, int room)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.LeaveGame;
            newCommand.playerRoom = room;
            newCommand.message = newCommand.cType + ":" + newCommand.playerRoom + ";";
            return newCommand;
        }

        public static Command leftPlayerCommand(long ts, string username, int n)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.LeftPlayer;
            newCommand.username = username;
            newCommand.playerNumber = n;
            newCommand.message = newCommand.cType + ":" + newCommand.username + ":" + newCommand.playerNumber + ";";
            return newCommand;
        }

        public static Command playerPositionCommand(long ts, Player p1, float x1, float y1, int d1)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.PlayerPosition;
            newCommand.message = newCommand.cType + ":" + ts + ":" + p1.playerNumber + ":" + x1 + ":" + y1 + ":" + d1 + ";";

            return newCommand;
        }

        public static Command eatPelletCommand(long ts, Player p1, int oldPelletID, Pellet newPellet)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.EatPellet;
            newCommand.message = newCommand.cType + ":" + ts + ":" + p1.playerNumber + ":" + p1.size + ":" + oldPelletID + ":" + newPellet.id + ":" + newPellet.x + ":" + newPellet.y + ":" + newPellet.size + ";";

            return newCommand;
        }

        public static Command eatPlayerCommand(long ts, Player p1, Player p2)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.EatPlayer;
            newCommand.message = newCommand.cType + ":" + ts + ":" + p1.playerNumber + ":" + p1.size + ":" + p2.playerNumber + ":" + p2.size + ":" + p2.x + ":" + p2.y + ":" + p2.dir + ";";

            return newCommand;
        }

        public static Command deathCommand(long ts, Player p1)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.Death;
            newCommand.message = newCommand.cType + ":" + ts + ":" + p1.playerNumber + ":" + p1.size + ":" + p1.x + ":" + p1.y + ":" + p1.dir + ";";

            return newCommand;
        }

        public static Command roomUpdateCommand(long ts, GameState gs)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.RoomUpdate;
            newCommand.message = newCommand.cType + ":" + ts + ":" + gs.roomID +":" + gs.playerList.Count + ";";

            return newCommand;
        }

        public static Command unwrap(string message)
        {
            //Console.WriteLine(message);
            string[] data = message.Split(new char[] {delimiter});
            foreach(string str in data)
            {
                Console.WriteLine(str);
            }
            Command newCommand = new Command();
            switch((CType)Enum.Parse(typeof(CType), data[0]))
            {
                case CType.Login:
                    newCommand.cType = CType.Login;
                    newCommand.username = data[1];
                    newCommand.password = data[2];
                    break;
                case CType.JoinGame:
                    newCommand.cType = CType.JoinGame;
                    newCommand.timeStamp = Convert.ToInt32(data[1]);
                    newCommand.playerRoom = Convert.ToInt32(data[2]);
                    break;
                case CType.LeaveGame:
                    newCommand.cType = CType.LeaveGame;
                    newCommand.timeStamp = long.Parse(data[1]);
                    newCommand.playerRoom = Convert.ToInt32(data[2]);
                    break;
                case CType.PlayerPosition:
                    newCommand.cType = CType.PlayerPosition;
                    newCommand.timeStamp = long.Parse(data[1]);
                    newCommand.x = float.Parse(data[2]);
                    newCommand.y = float.Parse(data[3]);
                    newCommand.dir = int.Parse(data[4]);
                    break;
                case CType.Disconnect:
                    newCommand.cType = CType.Disconnect;
                    break;
                default:
                    Console.WriteLine("Command receieved was invalid.");
                    break;
            }

            return newCommand;
        }
    }
}
