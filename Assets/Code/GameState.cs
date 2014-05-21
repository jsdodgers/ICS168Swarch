using UnityEngine;
using System.Collections;

namespace Swarch {
	public class GameState : MonoBehaviour {
		Global globalVariables;
		public Connection connection;

		public ArrayList players;
		public ArrayList pellets;
		public bool gameStarted;
		public GameObject playerPrefab;
		public GameObject pelletPrefab;
	
		// Use this for initialization
		void Start () {
			globalVariables = (Global)GameObject.FindObjectOfType(typeof(Global));
			connection = GameObject.Find("Connection").GetComponent<Connection>();
			gameStarted = false;
			players = new ArrayList();
			pellets = new ArrayList();
		}

		public void addPlayer(string playerName, int playerNum) {
			GameObject go = (GameObject)Instantiate(playerPrefab);
			go.renderer.enabled = false;
			Player p = go.GetComponent<Player>();
			players.Add(p);
			p.name = playerName;
			p.id = playerNum;
		}

		public void removePlayer(string playerName, int playerNum) {
			foreach (Player p in players) {
				if (p.name == playerName) {
					players.Remove(p);
					Destroy(p.gameObject);
				}
			}
		}

		public void startGame() {
			Debug.Log("StartGame();");
			gameStarted = true;
			foreach (Player p in players) {
				p.renderer.enabled = true;
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		
		void OnGUI()
		{
			float width = Screen.width;
			float height = Screen.height;
			TextAnchor t = GUI.skin.label.alignment;
			int fontSize = GUI.skin.label.fontSize;
			GUI.Label(new Rect(10, 10, 200, 20), globalVariables.GetPlayerName());
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			if (connection.currentRoom>0) {
				GUI.Label(new Rect(0,0,width,40),connection.rooms.get(connection.currentRoom).name);
			}
			GUI.skin.label.fontSize = 40;
			if (!gameStarted) {
				GUI.Label(new Rect(0,0,width,height),"Waiting for more players to join.");
			}
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.alignment = t;
			if (GUI.Button(new Rect(Screen.width-120,10,110,25),"Leave Game")) {
				connection.sendCommand(Command.LeaveGame(0,connection.currentRoom));
				//			Application.LoadLevel(0);
			}
		
		}

	}

}