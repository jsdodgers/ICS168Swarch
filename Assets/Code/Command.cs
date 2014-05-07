using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CType : byte {Login, StartGame, NewPlayer, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect}
public enum LoginResponseType : int {
	FailedLogin		= 0,
	SucceededLogin	= 1 << 0,
	NewUser			= 1 << 1
}

namespace Swarch {
	public class Command  {
		public string message, username, password;
		public CType cType;
		private long timeStamp;
		private int playerNumber;
		public int[] scores;
		private const char delimiter = ':';
		public LoginResponseType loginResponse;

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

		public static Command StartGame(long timeStamp) {
			Command comm = new Command();
			comm.timeStamp = timeStamp;
			comm.cType = CType.StartGame;
			comm.message = comm.cType + ";";
			return comm;
		}

		public static Command NewPlayer(long timestamp,string username, int playerNumber) {
			Command comm = new Command();
			comm.timeStamp = timestamp;
			comm.cType = CType.NewPlayer;
			comm.username = username;
			comm.playerNumber = playerNumber;
			comm.message = comm.cType + ":" + username + ":" + playerNumber+";";
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
				newCommand.loginResponse = (LoginResponseType)Enum.Parse(typeof(LoginResponseType),data[1]);
				break;
			case CType.StartGame:
				newCommand = new Command();
				newCommand.cType = CType.StartGame;
				break;
			case CType.NewPlayer:
				newCommand = new Command();
				newCommand.cType = CType.NewPlayer;
				newCommand.username = data[1];
				newCommand.playerNumber = Convert.ToInt32(data[2]);
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
