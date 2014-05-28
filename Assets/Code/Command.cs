using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum CType : byte {Login, StartGame, NewPlayer,LeftPlayer, PlayerPosition, SizeUpdate, EatPellet, SpawnPellet, EatPlayer, Death, Disconnect, JoinGame, LeaveGame, RoomUpdate}
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
		public int playerStartNum1;
		public int playerStartNum2;
		public float playerStartX1, playerStartX2, playerStartY1, playerStartY2, x, y;
		public int playerStartDir1,playerStartDir2, dir;
		public int[] playerNums,playerDirs;
		public float[] pelletsX, pelletsY, pelletsSize, playerXs,playerYs,playerSizes;
		public int[] pelletsId;
		public float playerNewSize;
		public int playerId, newPelletId, oldPelletId;
		public float pelletX, pelletY, pelletSize;
		public int eatenPlayerId,eatingPlayerId;
		public float playerX,playerY,eatenPlayerSize,eatingPlayerSize;
		public int type;
		public int numPlayers;

		public static Command PlayerPosition(long timeStamp, float xx, float yy, int dirr) {
			Command comm = new Command();
			comm.timeStamp = timeStamp;
			comm.cType = CType.PlayerPosition;
			comm.x = xx;
			comm.y = yy;
			comm.dir = dirr;
			comm.message = comm.cType + ":" + timeStamp + ":" + xx + ":" + yy + ":" + dirr + ";";
			return comm;
		}

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
				Debug.Log("StartGame: " + message);
				newCommand.cType = CType.StartGame;
				newCommand.type = int.Parse(data[1]);
				if (newCommand.type==0) {
					newCommand.playerStartNum1 = Convert.ToInt32(data[2]);
					newCommand.playerStartX1 = float.Parse(data[3]);
					newCommand.playerStartY1 = float.Parse(data[4]);
					newCommand.playerStartDir1 = int.Parse(data[5]);
					newCommand.playerStartNum2 = Convert.ToInt32(data[6]);
					newCommand.playerStartX2 = float.Parse(data[7]);
					newCommand.playerStartY2 = float.Parse(data[8]);
					newCommand.playerStartDir2 = int.Parse(data[9]);
					int num = (data.Length-10)/4;
					newCommand.pelletsX = new float[num];
					newCommand.pelletsY = new float[num];
					newCommand.pelletsSize = new float[num];
					newCommand.pelletsId = new int[num];
					for (int n=10;n<data.Length;n+=4) {
						newCommand.pelletsId[(n-10)/4] = int.Parse(data[n]);
						newCommand.pelletsX[(n-10)/4] = float.Parse(data[n+1]);
						newCommand.pelletsY[(n-10)/4] = float.Parse(data[n+2]);
						newCommand.pelletsSize[(n-10)/4] = float.Parse(data[n+3]);
					}
				}
				else if (newCommand.type==1) {
					int numPlayers = int.Parse(data[2]);
					newCommand.playerNums = new int[numPlayers];
					newCommand.playerXs = new float[numPlayers];
					newCommand.playerYs = new float[numPlayers];
					newCommand.playerSizes = new float[numPlayers];
					newCommand.playerDirs = new int[numPlayers];
					int curr = 3;
					for (int n=0;n<numPlayers;n++) {
						newCommand.playerNums[n] = int.Parse(data[curr]);curr++;
						newCommand.playerXs[n] = float.Parse(data[curr]);curr++;
						newCommand.playerYs[n] = float.Parse(data[curr]);curr++;
						newCommand.playerDirs[n] = int.Parse(data[curr]);curr++;
						newCommand.playerSizes[n] = float.Parse(data[curr]);curr++;
					}
					int numPellets = int.Parse(data[curr]);
					curr++;
					newCommand.pelletsX = new float[numPellets];
					newCommand.pelletsY = new float[numPellets];
					newCommand.pelletsSize = new float[numPellets];
					newCommand.pelletsId = new int[numPellets];
					for (int n=0;n<numPellets;n++) {
						Debug.Log(data[curr] + " " + data[curr+1] + " " + data[curr+2] + " " + data[curr+3]);
						newCommand.pelletsId[n] = int.Parse(data[curr]);curr++;
						newCommand.pelletsX[n] = float.Parse(data[curr]);curr++;
						newCommand.pelletsY[n] = float.Parse(data[curr]);curr++;
						newCommand.pelletsSize[n] = float.Parse(data[curr]);curr++;
					}
				}
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
			case CType.PlayerPosition:
				newCommand.cType = CType.PlayerPosition;
				newCommand.timeStamp = long.Parse(data[1]);
				newCommand.playerNumber = int.Parse(data[2]);
				newCommand.x = float.Parse(data[3]);
				newCommand.y = float.Parse(data[4]);
				newCommand.dir = int.Parse(data[5]);
				break;
			case CType.EatPellet:
				newCommand.cType = CType.EatPellet;
				newCommand.timeStamp = long.Parse(data[1]);
				newCommand.playerId = int.Parse(data[2]);
				newCommand.playerNewSize = float.Parse(data[3]);
				newCommand.oldPelletId = int.Parse(data[4]);
				newCommand.newPelletId = int.Parse(data[5]);
				newCommand.pelletX = float.Parse(data[6]);
				newCommand.pelletY = float.Parse(data[7]);
				newCommand.pelletSize = float.Parse(data[8]);
				break;
			case CType.EatPlayer:
				newCommand.cType = CType.EatPlayer;
				newCommand.timeStamp = long.Parse(data[1]);
				newCommand.eatingPlayerId = int.Parse(data[2]);
				newCommand.eatingPlayerSize = float.Parse(data[3]);
				newCommand.eatenPlayerId = int.Parse(data[4]);
				newCommand.eatenPlayerSize = float.Parse(data[5]);
				newCommand.playerX = float.Parse(data[6]);
				newCommand.playerY = float.Parse(data[7]);
				newCommand.dir = int.Parse(data[8]);
				break;
			case CType.Death:
				newCommand.cType = CType.Death;
				newCommand.timeStamp = long.Parse(data[1]);
				newCommand.playerId = int.Parse(data[2]);
				newCommand.playerNewSize = float.Parse(data[3]);
				newCommand.playerX = float.Parse(data[4]);
				newCommand.playerY = float.Parse(data[5]);
				newCommand.dir = int.Parse(data[6]);
				break;
			case CType.RoomUpdate:
				Debug.Log("RoomUpdate:  " + message);
				newCommand.cType = CType.RoomUpdate;
				newCommand.timeStamp = long.Parse(data[1]);
				newCommand.roomNum = int.Parse(data[2]);
				newCommand.numPlayers = int.Parse(data[3]);
				break;
			default:
				break;
			}
			
			return newCommand;
		}
	}
}
