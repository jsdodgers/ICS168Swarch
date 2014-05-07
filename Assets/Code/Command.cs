using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CType : byte {Login, StartGame, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect}

namespace Swarch {
	public class Command  {
		public string message, username, password;
		public CType cType;
		private long timeStamp;
		private int playerNumber;
		public int[] scores;
		private const char delimiter = ':';

		public static Command Login(long timeStamp, string username, string password)
		{
			Command comm = new Command();
			comm.timeStamp = timeStamp;
			comm.cType = CType.Login;
			comm.username = username;
			comm.password = password;
			comm.message = comm.cType + ":"+username+":"+password+";";
			return comm;
		}


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
