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

		public void setPlayerPosition(int playerNum, float x, float y, int dir) {
			foreach (Player p in players) {
				if (p.id==playerNum) {
					p.setDirection(dir);
					p.transform.position = new Vector3(x,y,0);
				}
			}
		}

		public void addPlayer(string playerName, int playerNum) {
			GameObject go = (GameObject)Instantiate(playerPrefab);
			go.renderer.enabled = false;
			Player p = go.GetComponent<Player>();
			players.Add(p);
			p.name = playerName;
			p.id = playerNum;
			p.isSelf = (playerName==globalVariables.GetPlayerName());
		}

		public void removePlayer(string playerName, int playerNum) {
			ArrayList players3 = new ArrayList();
			foreach (Player p in players3) players3.Add(p);
			foreach (Player p in players3) {
				if (p.name == playerName) {
					players.Remove(p);
					Destroy(p.gameObject);
				}
			}
		}

		public void startGame(int player1, float x1, float y1, int d1, int player2, float x2, float y2, int d2) {
			gameStarted = true;
			foreach (Player p in players) {
				Debug.Log("Player " + p.name + ": " + p.id);
				if (p.id == player1) {
					p.transform.position = new Vector3(x1,y1,0);
					p.setDirection(d1);
				}
				else if (p.id==player2) {
					p.transform.position = new Vector3(x2,y2,0);
					p.setDirection(d2);
				}
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