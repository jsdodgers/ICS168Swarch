using UnityEngine;
using System.Collections;
using System.Threading;


namespace Swarch {
	public class Login : MonoBehaviour
	{

		public Global globalVariables;
		public string player1Name, player1Password;
		public int textPositionX, textPositionY;
		public Connection connection;
		ArrayList playerNames = new ArrayList();
		public int error;
		bool remember;
		bool loggingIn;
		string connectionString = "";
		// Use this for initialization
		void Start()
		{
			connection = GameObject.Find("Connection").GetComponent<Connection>();
			connection.socks.SERVER_LOCATION = PlayerPrefs.GetString("IP");
			error = 0;
			loggingIn = false;
			remember = PlayerPrefs.GetInt("remember")==1;
			if (remember) player1Name = PlayerPrefs.GetString("playerName");
		}
		
		// Update is called once per frame
		void Update()
		{
		
		}

		void OnGUI()
		{
			if (GUI.GetNameOfFocusedControl()=="password" || player1Password!="") {
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
					if (player1Name!="") {
						login();
					}
				}
			}
			float width = Screen.width;
			float height = Screen.height;
			textPositionX = (int)(width/2 - (65 + 200)/2);
			textPositionY = (int)(height/2 - (25 + 25 + 20)/2);
			GUI.Label(new Rect(textPositionX, textPositionY, 200, 20), "Username: ");
			GUI.Label(new Rect(textPositionX, GetPasswordY(), 200, 20), "Password: ");
			GUI.Label(new Rect(textPositionX, GetIPY(),200, 20), "IP: ");


			float errorPositionX = GetInputX();
			float errorPositionY = GetFailureY();
			TextAnchor a = GUI.skin.label.alignment;
			if (error == 1) {
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(errorPositionX,errorPositionY,200,20),"Incorrect Password.");
			}
			else if (error == 2) {
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label(new Rect(errorPositionX,errorPositionY,200,20),"Enter username and password.");
			}
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(errorPositionX,GetConnectionY(),200,20),connectionString);
			GUI.skin.label.alignment = a;

			player1Name = GUI.TextField(new Rect(GetInputX(), textPositionY, 200, 20), player1Name);
			connection.socks.SERVER_LOCATION = GUI.TextField(new Rect(GetInputX(),GetIPY(),200,20),connection.socks.SERVER_LOCATION);
			PlayerPrefs.SetString("IP",connection.socks.SERVER_LOCATION);
			remember = GUI.Toggle(new Rect(GetInputX()+205,textPositionY,200,20),remember,"Remember Username");
			if (remember) PlayerPrefs.SetString("playerName",player1Name);
			else PlayerPrefs.SetString("playerName","");
			PlayerPrefs.SetInt("remember",(remember?1:0));
			GUI.SetNextControlName("password");
			player1Password = GUI.PasswordField(new Rect(GetInputX(), GetPasswordY(), 200, 20), player1Password, '*');
			if(GUI.Button(new Rect(GetInputX(), GetLoginY(), 95, 20), "Login"))
			{
				login();
			}

			if(GUI.Button(new Rect(GetInputX() + 105, GetLoginY(), 95, 20), "Quit"))
			{
				Application.Quit();
			}
		}

		void login() {
			loggingIn = true;
			connectionString = "Connecting...";
			Thread t = new Thread(new ThreadStart(connectToServer));
			t.Start();
			SQLiteDB db = new SQLiteDB();
			if (db.dbConnect("users.sqlite")) {
				if (player1Name!="" && player1Password!="") {
					string p1 = hashedPass();
					string pass = db.getPassword(player1Name);
					if (pass==null) {
						pass = p1;
						db.addUser(player1Name,p1);
					}
					if (pass == p1) {
						db.dbPrintAll();
						globalVariables.SetPlayerName(player1Name);
			//			Application.LoadLevel(0);
					}
					else {
						error = 1;
					}
				}
				else {
					error = 2;
				}
			}
		}

		void connectToServer() {
			if (connection.socks.Connect()) {
				connectionString = "Connect Succeeded";
			}
			else {
				connectionString = "Connect Failed";
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
