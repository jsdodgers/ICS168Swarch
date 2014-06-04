using UnityEngine;
using System.Collections;
using System;
using System.Text;

namespace Swarch {
	public class Connection : MonoBehaviour {
		
		public Sockets socks;
		public Rooms rooms;
		public int currentRoom;
	//	public Sockets socks;

		//	private byte byteBuffer;
		//	private byte tempBuffer;

		
		public bool loggingIn;
		public bool loggedIn;
		public bool connected;

		public int player_num;
		public bool gameStarted;
		//	public Ball ball;
		
		//	public GUIS gui;
		
	//	DateTime dt;
	//	System.Diagnostics.Stopwatch uniClock;

		// Use this for initialization

		public void sendCommand(Command comm) {
			byte[] bytes = Encoding.UTF8.GetBytes(comm.message);
			socks.SendTCPPacket(bytes);
		}
			
		void OnApplicationQuit() {
			Command comm = Command.Disconnect(0);
			byte[] bytes = Encoding.UTF8.GetBytes(comm.message);
			socks.SendTCPPacket(bytes);
		}

		void Start () {

			DontDestroyOnLoad(gameObject);
			loggingIn = false;
			connected = false;
			loggedIn = false;

			gameStarted = false;
			rooms = new Rooms();
			socks = (Sockets)gameObject.AddComponent("Sockets");
			socks.SERVER_LOCATION = PlayerPrefs.GetString("IP");
		//	uniClock = new System.Diagnostics.Stopwatch();
		//	dt = NTPTime.getNTPTime(ref uniClock);
			
		}
		
		// Update is called once per frame
		void Update () {
			lock(socks.recvBuffer) {
				ArrayList addBack = new ArrayList();
				while (socks.recvBuffer.Count !=0) {
					string curr = (string)socks.recvBuffer.Dequeue();
					Command comm = Command.unwrap(curr);
					GameState gs;
					switch(comm.cType) {
					case CType.Login:
						LoginResponseType type = comm.loginResponse;
						Login loginScreen = GameObject.Find("Login").GetComponent<Login>();
						if (!((type & LoginResponseType.NewUser)==LoginResponseType.FailedLogin)) {
							loginScreen.newUser();
						}
						if (!((type & LoginResponseType.SucceededLogin)==LoginResponseType.FailedLogin)) {
							loginScreen.succeededConnection();
							for (int n=0;n<comm.numRooms;n++) {
								rooms.addRoom(comm.roomNames[n],comm.roomNums[n],n);
							}
						//	rooms.printRoomsToConsole();
						}
						if (type==LoginResponseType.FailedLogin) {
							loginScreen.failedConnection();
						}
						break;
					case CType.NewPlayer:
						string playerName = comm.username;
					//	Login loginScreen2 = GameObject.Find("Login").GetComponent<Login>();
					//	loginScreen2.playerNames.Add(playerName);
						if (Application.loadedLevel==1) {
							gs = GameObject.Find("GameState").GetComponent<GameState>();
							gs.addPlayer(playerName,comm.playerNumber);
						}
						else {
							addBack.Add(curr);
						}
						break;
//					case CType.StartGame:
//						Application.LoadLevel(1);
//						break;
					case CType.LeftPlayer:
						if (Application.loadedLevel==1) {
							string playerName1 = comm.username;
							int playerNum1 = comm.playerNumber;
							GameState gs1 = GameObject.Find("GameState").GetComponent<GameState>();
							gs1.removePlayer(playerName1,playerNum1);
						}
						break;
					case CType.JoinGame:
						currentRoom = comm.roomNum;
						Application.LoadLevel(1);
						break;
					case CType.LeaveGame:
						currentRoom = -1;
						Application.LoadLevel(0);
						break;
					case CType.StartGame:
						if (Application.loadedLevel==1) {
							gs = GameObject.Find("GameState").GetComponent<GameState>();
							if (comm.type==0) {
								gs.startGame(comm.playerStartNum1,comm.playerStartX1,comm.playerStartY1,comm.playerStartDir1,comm.playerStartNum2,comm.playerStartX2,comm.playerStartY2,comm.playerStartDir2,comm.pelletsId,comm.pelletsX,comm.pelletsY,comm.pelletsSize);
							}
							else if (comm.type==1) {
								gs.startGame(comm.playerNums,comm.playerXs,comm.playerYs,comm.playerSizes,comm.playerDirs,comm.pelletsId,comm.pelletsX,comm.pelletsY,comm.pelletsSize);
							}
						}
						else {
							addBack.Add(curr);
						}
						break;
					case CType.PlayerPosition:
						if (Application.loadedLevel==1) {
							gs = GameObject.Find("GameState").GetComponent<GameState>();
							gs.setPlayerPosition(comm.playerNumber,comm.x,comm.y,comm.dir);
						}
						break;
					case CType.EatPellet:
						if (Application.loadedLevel==1) {
							gs = GameObject.Find("GameState").GetComponent<GameState>();
							gs.removePellet(comm.oldPelletId);
							gs.addPellet(comm.newPelletId,comm.pelletX,comm.pelletY,comm.pelletSize);
							gs.setPlayerSize(comm.playerId, comm.playerNewSize);
							gs.addPlayerScore(comm.playerId,1);
						}
						break;
					case CType.EatPlayer:
						if (Application.loadedLevel==1) {
							gs = GameObject.Find("GameState").GetComponent<GameState>();
							gs.setPlayerSize(comm.eatingPlayerId, comm.eatingPlayerSize);
							gs.setPlayerSize(comm.eatenPlayerId, comm.eatenPlayerSize);
							gs.setPlayerPosition(comm.eatenPlayerId,comm.playerX,comm.playerY,comm.dir);
							gs.addPlayerScore(comm.eatingPlayerId,10);
						}
						break;
					case CType.Death:
						if (Application.loadedLevel==1) {
							gs = GameObject.Find("GameState").GetComponent<GameState>();
							gs.setPlayerSize(comm.playerId, comm.playerNewSize);
							gs.setPlayerPosition(comm.playerId,comm.playerX,comm.playerY,comm.dir);
						}
						break;
					case CType.RoomUpdate:
						Room r = rooms.getWithId(comm.roomNum);
						if (r!=null)
							r.numPlayers = comm.numPlayers;
						break;
					default:
						break;
					}
				}
				foreach (string str in addBack) {
					socks.recvBuffer.Enqueue(str);
				}
			}
		}
		/*
		public void setOtherPaddlePos(float pos) {
			Paddle p;
			if (player_num == 0) {
				p = GameObject.Find("Paddle-Right").GetComponent<Paddle>();
			}
			else {
				p = GameObject.Find("Paddle-Left").GetComponent<Paddle>();
			}
			p.transform.position = new Vector3(p.transform.position.x,pos,p.transform.position.z);
		}
		*/
	//	public long getTimeStamp() {
	//		return (dt.AddMinutes(uniClock.Elapsed.Minutes).AddSeconds(uniClock.Elapsed.Seconds).AddMilliseconds(uniClock.Elapsed.Milliseconds)).Ticks;
	//	}
		
	}
}