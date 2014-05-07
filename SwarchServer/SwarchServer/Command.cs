using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarchServer
{
    public enum CType : byte {Login, StartGame, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect}
    
    class Command
    {
        public string message, username, password;
        public CType cType;
        private long timeStamp;
        private int playerNumber;
        public int[] scores;
        private const char delimiter = ':';

        public static Command unwrap(string message)
        {
            string[] data = message.Split(new char[] {delimiter});
            Command newCommand;
            switch((CType)Enum.Parse(typeof(CType), data[0]))
            {
                case CType.Login:
                    newCommand = new Command();
                    newCommand.cType = CType.Login;
                    newCommand.username = data[1];
                    newCommand.password = data[2];
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
