using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarchServer
{
    public enum CType : byte {Login, StartGame, NewPlayer, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect}
    public enum LoginResponseType : int {FailedLogin = 0, SucceededLogin = 1 << 0, NewUser = 1 << 1}

    class Command
    {
        public string message, username, password;
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

        public static Command startGameCommand(long ts)
        {
            Command newCommand = new Command();
            newCommand.timeStamp = ts;
            newCommand.cType = CType.StartGame;
            newCommand.message = newCommand.cType + ";";

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

        public static Command unwrap(string message)
        {
            //Console.WriteLine(message);
            string[] data = message.Split(new char[] {delimiter});
            /*foreach(string str in data)
            {
                Console.WriteLine(str);
            }*/
            Command newCommand;
            switch((CType)Enum.Parse(typeof(CType), data[0]))
            {
                case CType.Login:
                    newCommand = new Command();
                    newCommand.cType = CType.Login;
                    newCommand.username = data[1];
                    newCommand.password = data[2];
                    break;
                case CType.Disconnect:
                    newCommand = new Command();
                    newCommand.cType = CType.Disconnect;
                    break;
                default:
                    Console.WriteLine("Command receieved was invalid.");
                    newCommand = new Command();
                    break;
            }

            return newCommand;
        }
    }
}
