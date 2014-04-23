using UnityEngine;
using System.Collections;

public class Login : MonoBehaviour
{
	public Global globalVariables;
	public string player1Name, player1Password;
	public int textPositionX, textPositionY;
	ArrayList playerNames = new ArrayList();
	
	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	void OnGUI()
	{
		float width = Screen.width;
		float height = Screen.height;
		textPositionX = (int)(width/2 - (65 + 200)/2);
		textPositionY = (int)(height/2 - (25 + 25 + 20)/2);
		GUI.Label(new Rect(textPositionX, textPositionY, 200, 20), "Username: ");
		GUI.Label(new Rect(textPositionX, GetPasswordY(), 200, 20), "Password: ");


	

		player1Name = GUI.TextField(new Rect(GetInputX(), textPositionY, 200, 20), player1Name);
		player1Password = GUI.PasswordField(new Rect(GetInputX(), GetPasswordY(), 200, 20), player1Password, '*');
		if(GUI.Button(new Rect(GetInputX(), GetLoginY(), 95, 20), "Login"))
		{
			globalVariables.SetPlayerName(player1Name);
			Application.LoadLevel(0);
		}

		if(GUI.Button(new Rect(GetInputX() + 105, GetLoginY(), 95, 20), "Quit"))
		{
			Application.Quit();
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
