using UnityEngine;
using System.Collections;
using System;

namespace Swarch {
	public class Connection : MonoBehaviour {
		
		public Sockets socks;
	//	public Sockets socks;

		//	private byte byteBuffer;
		//	private byte tempBuffer;
		
		public int player_num;
		public bool connected;
		public bool gameStarted;
		//	public Ball ball;
		
		//	public GUIS gui;
		
	//	DateTime dt;
	//	System.Diagnostics.Stopwatch uniClock;

		// Use this for initialization
		void Start () {
			connected = false;
			gameStarted = false;
			socks = (Sockets)gameObject.AddComponent("Sockets");
		//	uniClock = new System.Diagnostics.Stopwatch();
		//	dt = NTPTime.getNTPTime(ref uniClock);
			
		}
		
		// Update is called once per frame
		void Update () {
		//	lock(socks.recvBuffer) {
		//		while (socks.recvBuffer.Count !=0) {
					/*
				string curr = (string)socks.recvBuffer.Dequeue();
				Command comm = Command.unwrap(curr);
				Debug.Log("Command type: " + comm.commandType);
				switch (comm.commandType) {
					case CommandType.Initialize:
						player_num = comm.playerNumber;
						connected = true;
						break;
					case CommandType.StartGame:
						gameStarted = true;
						break;
					case CommandType.PaddleUpdate:
						Debug.Log("Paddle Pos1:  " + comm.paddlePosition);
						setOtherPaddlePos(comm.paddlePosition);
						break;
					case CommandType.BallVelocity:
						ball.velocity = new Vector2(comm.ballVelocityX,comm.ballVelocityY);
						break;
					case CommandType.BallPosition:
						ball.transform.position = new Vector3(comm.ballPositionX,comm.ballPositionY,ball.transform.position.z);
						break;
					case CommandType.ScoreUpdate:
						gui.player1Score = (int)comm.scores[0];
						gui.player2Score = (int)comm.scores[1];
						break;
					default:
						break;
					
				}*/
		//		}
		//	}
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