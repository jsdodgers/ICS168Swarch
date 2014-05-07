using UnityEngine;
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
		bool loggingIn;
		bool loggedIn;
		bool connected;
		bool newAccount;
		string connectionString = "";
		// Use this for initialization
		void Start()
		{
			connection = GameObject.Find("Connection").GetComponent<Connection>();
			connection.socks.SERVER_LOCATION = PlayerPrefs.GetString("IP");
			error = 0;
			loggingIn = false;
			connected = false;
			loggedIn = false;
			newAccount = false;
			remember = PlayerPrefs.GetInt("remember")==1;
			if (remember) player1Name = PlayerPrefs.GetString("playerName");
		}
		
		// Update is called once per frame
		void Update()
		{
		
		}

		void OnGUI()
		{
			float width = Screen.width;
			float height = Screen.height;

			if (!loggedIn) {
			if (GUI.GetNameOfFocusedControl()=="password" || player1Password!="") {
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
					if (player1Name!="" && !loggingIn) {
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
			GUI.enabled = !loggingIn;
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
			else {
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
				}
			}
		}

		void login() {
			if (!connected) {
				loggingIn = true;
				connectionString = "Connecting...";
				Thread t = new Thread(new ThreadStart(connectToServer));
				t.Start();
			}
			else {
				actualLogin();
			}
		}

		public void succeededConnection() {
			loggedIn = true;
			loggingIn = false;
			error = 0;
		}

		public void failedConnection() {
			loggingIn = false;
			error = 1;
		}

		public void newUser() {
			newAccount = true;
		}

		void actualLogin() {
//			Command comm = Command.PaddleUpdate(gameProcess.getTimeStamp(),this);
//			byte[] bytes = Encoding.UTF8.GetBytes(comm.message + ";");
//			gameProcess.socks.SendTCPPacket(bytes);
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
				connected = true;
			}
			else {
				connectionString = "Connect Failed";
				loggingIn = false;
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
