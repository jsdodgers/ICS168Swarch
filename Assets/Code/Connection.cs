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
					Debug.Log(comm.cType);
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
						Debug.Log(Application.loadedLevel);
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
							gs.startGame(comm.playerStartNum1,comm.playerStartX1,comm.playerStartY1,comm.playerStartNum2,comm.playerStartX2,comm.playerStartY2);
						}
						else {
							addBack.Add(curr);
						}
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