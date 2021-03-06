﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Text;


namespace Swarch {
	public class Login : MonoBehaviour
	{

		public Global globalVariables;
		public string player1Name, player1Password;
		public int textPositionX, textPositionY;
		public Connection connection;
		public ArrayList playerNames = new ArrayList();
		public int error;
		bool remember;
		bool rememberPass;
		bool newAccount;
		string connectionString = "";
		string[] usernames;
		int[] scores;
		int yourScore;
		int yourRank;
		public bool showingScores;
		// Use this for initialization
		void Start()
		{
			connection = GameObject.Find("Connection").GetComponent<Connection>();
			error = 0;
			newAccount = false;
			remember = PlayerPrefs.GetInt("remember")==1;
			if (remember) player1Name = PlayerPrefs.GetString("playerName");
			rememberPass = PlayerPrefs.GetInt("rememberPass")==1;
			if (rememberPass)player1Password = PlayerPrefs.GetString("playerPass");
		}
		
		// Update is called once per frame
		void Update()
		{
		
		}
		
		void OnGUI()
		{
			float width = Screen.width;
			float height = Screen.height;
			
			if (!connection.loggedIn) {
				if (GUI.GetNameOfFocusedControl()=="password" || player1Password!="") {
					if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
						if (player1Name!="" && !connection.loggingIn) {
							login();
						}
					}
				}
				textPositionX = (int)(width/2 - (65 + 200)/2);
				textPositionY = (int)(height/2 - (25 + 25 + 20)/2);
				GUI.Label(new Rect(textPositionX, textPositionY, 200, 20), "Username: ");
				GUI.Label(new Rect(textPositionX, GetPasswordY(), 200, 20), "Password: ");
				GUI.Label(new Rect(textPositionX, GetIPY(),200, 20), "IP: ");
				
				
				float errorPositionX = GetInputX();
				float errorPositionY = GetFailureY();
				TextAnchor a = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				if (error == 1) {
					GUI.Label(new Rect(errorPositionX,errorPositionY,200,20),"Incorrect Password.");
				}
				else if (error == 2) {
					GUI.Label(new Rect(errorPositionX,errorPositionY,200,20),"Enter username and password.");
				}
				GUI.Label(new Rect(errorPositionX,GetConnectionY(),200,20),connectionString);
				GUI.skin.label.alignment = a;
				
				
				connection.socks.SERVER_LOCATION = GUI.TextField(new Rect(GetInputX(),GetIPY(),200,20),connection.socks.SERVER_LOCATION);
				player1Name = GUI.TextField(new Rect(GetInputX(), textPositionY, 200, 20), player1Name);
				PlayerPrefs.SetString("IP",connection.socks.SERVER_LOCATION);
				remember = GUI.Toggle(new Rect(GetInputX()+205,textPositionY,200,20),remember,"Remember Username");
				if (remember) PlayerPrefs.SetString("playerName",player1Name);
				else PlayerPrefs.SetString("playerName","");
				PlayerPrefs.SetInt("remember",(remember?1:0));
				GUI.SetNextControlName("password");
				player1Password = GUI.PasswordField(new Rect(GetInputX(), GetPasswordY(), 200, 20), player1Password, '*');
				rememberPass = GUI.Toggle(new Rect(GetInputX()+205,GetPasswordY(),200,20),rememberPass,"Remember Password");
				if (rememberPass)PlayerPrefs.SetString("playerPass",player1Password);
				else PlayerPrefs.SetString("playerPass","");
				PlayerPrefs.SetInt("rememberPass",(rememberPass?1:0));
				GUI.enabled = !connection.loggingIn;
				if(GUI.Button(new Rect(GetInputX(), GetLoginY(), 95, 20), "Login"))
				{
					login();
				}
				GUI.enabled = true;
				if(GUI.Button(new Rect(GetInputX() + 105, GetLoginY(), 95, 20), "Quit"))
				{
					Application.Quit();
				}
				
			}
			else if (!showingScores) {

				int numRooms = connection.rooms.count();
				float sidePerc = .07f;
				float divPerc = .03f;
				float buttonPerc = .15f;
				float topBotPerc = (1 - (numRooms * buttonPerc + (numRooms-1) * divPerc))/2;
				int oldSize = GUI.skin.label.fontSize;
				GUI.skin.label.fontSize = 32;
				TextAnchor ta = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(0,0,width,topBotPerc*height),"Rooms");
				GUI.skin.label.fontSize = oldSize;
				GUI.skin.label.alignment = ta;
				for (int n=0;n<numRooms;n++) {
					Room r = connection.rooms.get(n);
					if (GUI.Button(new Rect(sidePerc * width,topBotPerc * height + (divPerc + buttonPerc) * n*height,(1-sidePerc*2)*width,buttonPerc*height),r.name + "      " + r.numPlayers + " Players")) {
//						Command comm = Command.JoinGame(0,r.id);
						connection.sendCommand(Command.JoinGame(0,r.id));
					}
				}
				if (GUI.Button(new Rect(width - 140.0f,15.0f, 125.0f,30.0f),"Logout")) {
					disconnect();
				}
				if (GUI.Button(new Rect(140.0f,15.0f,125.0f,30.0f),"High Scores")) {
					getHighScores();
				}
				/*
				int old = GUI.skin.label.fontSize;
				GUI.skin.label.fontSize = 30;
				TextAnchor a = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(width/2 - 200,height/2 - 100,400,200),(newAccount?"Account Created!\n":"")+"Logged in.\nWaiting for game to start.");
				GUI.skin.label.alignment = a;
				GUI.skin.label.fontSize = 16;
				for (int n=0;n<playerNames.Count;n++) {
					GUI.Label(new Rect(5,5+20*n,200,29),(n+1) + ". " + playerNames[n]);
				}
				GUI.skin.label.fontSize = old;
				if(GUI.Button(new Rect(width/2 + 120,height/2 - 70, 95, 20), "Quit"))
				{
					Application.Quit();
				}*/
			}
			else {
				float scoreWidth = 250;
				float scoreHeight = 20;
				TextAnchor a = GUI.skin.label.alignment;
				for (int n=0;n<scores.Length;n++) {
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUI.Label(new Rect((width - scoreWidth)/2,50 + scoreHeight*n,scoreWidth,scoreHeight),(n+1) + ". " + usernames[n]);
					GUI.skin.label.alignment = TextAnchor.MiddleRight;
					GUI.Label(new Rect((width - scoreWidth)/2,50 + scoreHeight*n,scoreWidth,scoreHeight),"" + scores[n]);
				}

				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.Label(new Rect((width - scoreWidth)/2,50 + scoreHeight *(scores.Length + 1),scoreWidth,scoreHeight),yourRank + ". " + player1Name);
				GUI.skin.label.alignment = TextAnchor.MiddleRight;
				GUI.Label(new Rect((width - scoreWidth)/2,50 + scoreHeight*(scores.Length + 1),scoreWidth,scoreHeight),"" + yourScore);
				GUI.skin.label.alignment = a;
				if (GUI.Button(new Rect(width - 140.0f,15.0f, 125.0f,30.0f),"Logout")) {
					disconnect();
				}
				if (GUI.Button(new Rect(140.0f,15.0f,125.0f,30.0f),"Rooms")) {
					leaveHighScores();
				}
			}
		}
		void leaveHighScores() {
			this.showingScores = false;
		}
		void getHighScores() {
			connection.sendCommand(Command.HighScore(0));
		}

		public void setHighScores(int[] scores,string[] playerNames, int rank, int yourScore) {
			this.scores = scores;
			this.usernames = playerNames;
			this.yourRank = rank;
			this.yourScore = yourScore;
			this.showingScores = true;
		}

		void disconnect() {
			this.showingScores = false;
			Command comm = Command.Disconnect(0);
			byte[] bytes = Encoding.UTF8.GetBytes(comm.message);
			connection.socks.SendTCPPacket(bytes);
			connection.loggedIn = false;
			if (connection.connected) {
				if (connection.socks.Disconnect()) connection.connected = false;
			}
		}

		void login() {
			if (!connection.connected) {
				connection.loggingIn = true;
				connectionString = "Connecting...";
				Thread t = new Thread(new ThreadStart(connectToServer));
				t.Start();
			}
			else {
				actualLogin();
			}
		}
		
		public void succeededConnection() {
			connection.loggedIn = true;
			connection.loggingIn = false;
			globalVariables.SetPlayerName(player1Name);
			error = 0;
		}
		
		public void failedConnection() {
			connection.loggingIn = false;
			error = 1;
		}
		
		public void newUser() {
			newAccount = true;
		}
		
		void actualLogin() {
			if (player1Name!="" && player1Password!="") {
				Command comm = Command.Login(0,player1Name,hashedPass());
				byte[] bytes = Encoding.UTF8.GetBytes(comm.message);
				connection.socks.SendTCPPacket(bytes);
			}
			else {
				error = 2;
			}
		}
		
		void connectToServer() {
			if (connection.socks.Connect()) {
				connectionString = "Connect Succeeded";
				actualLogin();
				connection.connected = true;
			}
			else {
				connectionString = "Connect Failed";
				connection.loggingIn = false;
			}
		}
		
		string hashedPass() {
			System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
			byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(player1Password));
			System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
			for (int n=0;n<data.Length;n++) {
				sBuilder.Append(data[n].ToString("x2"));
			}
			return sBuilder.ToString();
		}
		
		int GetInputX()
		{
			return (textPositionX + 65);
		}
		
		int GetPasswordY()
		{
			return (textPositionY + 25);
		}
		
		int GetLoginY()
		{
			return (GetPasswordY() + 25);
		}
		
		int GetConnectionY()
		{
			return GetLoginY() + 25;
		}
		
		int GetIPY()
		{
			return textPositionY - 25;
		}
		
		int GetFailureY()
		{
			;
			return GetIPY() - 25;
		}
	}
}
