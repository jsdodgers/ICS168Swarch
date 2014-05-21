using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CType : byte {Login, StartGame, NewPlayer,LeftPlayer, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect, JoinGame, LeaveGame}
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
		public int playerNumber;
		public int[] scores;
		private const char delimiter = ':';
		public LoginResponseType loginResponse;
		public int numRooms, roomNum;
		public string[] roomNames;
		public int[] roomNums;

		public static Command Disconnect(long timeStamp) {
			Command comm = new Command();
			comm.timeStamp = timeStamp;
			comm.cType = CType.Disconnect;
			comm.message = comm.cType + ":" + timeStamp + ";";
			return comm;
		}

		public static Command JoinGame(long timestamp, int roomId) {
			Command comm = new Command();
			comm.timeStamp = timestamp;
			comm.cType = CType.JoinGame;
			comm.roomNum = roomId;
			comm.message = comm.cType + ":" + timestamp + ":" + roomId + ";";
			return comm;
		}

		public static Command LeaveGame(long timestamp, int roomid) {
			Command comm = new Command();
			comm.timeStamp = timestamp;
			comm.cType = CType.LeaveGame;
			comm.roomNum = roomid;
			comm.message = comm.cType + ":" + timestamp + ":" + roomid + ";";
			return comm;
		}

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
			Command newCommand = new Command();
			switch((CType)Enum.Parse(typeof(CType), data[0]))
			{
			case CType.Login:
				newCommand.cType = CType.Login;
				newCommand.loginResponse = (LoginResponseType)Enum.Parse(typeof(LoginResponseType),data[1]);
				if ((newCommand.loginResponse&LoginResponseType.SucceededLogin)!=0) {
					newCommand.numRooms = Convert.ToInt32(data[2]);
					newCommand.roomNames = new string[newCommand.numRooms];
					newCommand.roomNums = new int[newCommand.numRooms];
					for (int n=0;n<newCommand.numRooms;n++) {
						newCommand.roomNames[n] = data[3+n*2];
						newCommand.roomNums[n] = Convert.ToInt32(data[4+n*2]);
					}
				}
				break;
			case CType.StartGame:
				newCommand.cType = CType.StartGame;
				break;
			case CType.NewPlayer:
				newCommand.cType = CType.NewPlayer;
				newCommand.username = data[1];
				newCommand.playerNumber = Convert.ToInt32(data[2]);
				break;
			case CType.LeftPlayer:
				newCommand.cType = CType.LeftPlayer;
				newCommand.username = data[1];
				newCommand.playerNumber = Convert.ToInt32(data[2]);
				break;
			case CType.JoinGame:
				newCommand.cType = CType.JoinGame;
				newCommand.roomNum = Convert.ToInt32(data[1]);
				break;
			case CType.LeaveGame:
				newCommand.cType = CType.LeaveGame;
				newCommand.roomNum = Convert.ToInt32(data[1]);
				break;
			default:
				Console.WriteLine("Command receieved was invalid.");
				break;
			}
			
			return newCommand;
		}
	}
}
