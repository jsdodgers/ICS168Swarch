using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour
{
	public Global globalVariables;
	public string player1Name, player1Password;
	public int textPositionX, textPositionY;
	ArrayList playerNames = new ArrayList();
	public int error;

	// Use this for initialization
	void Start()
	{
		error = 0;
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


		if (error == 1) {
			float errorPositionX = GetInputX();
			float errorPositionY = textPositionY - 25;
			TextAnchor a = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(errorPositionX,errorPositionY,200,20),"Incorrect Password.");
			GUI.skin.label.alignment = a;
		}
		else if (error == 2) {
			float errorPositionX = GetInputX();
			float errorPositionY = textPositionY - 25;
			TextAnchor a = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect(errorPositionX,errorPositionY,200,20),"Enter username and password.");
			GUI.skin.label.alignment = a;
		}
	

		player1Name = GUI.TextField(new Rect(GetInputX(), textPositionY, 200, 20), player1Name);
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
		SQLiteDB db = new SQLiteDB();
		if (db.dbConnect("users.sqlite")) {
			if (player1Name!="" && player1Password!="") {
				string pass = db.getPassword(player1Name);
				if (pass==null) {
					pass = player1Password;
					db.addUser(player1Name,player1Password);
				}
				if (pass == player1Password) {
					db.dbPrintAll();
					globalVariables.SetPlayerName(player1Name);
					Application.LoadLevel(0);
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
}
