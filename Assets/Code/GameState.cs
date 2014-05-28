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

		public void setPlayerSize(int playerNum,float size) {
			foreach (Player p in players) {
				if (p.id==playerNum) {
					p.setSize(size);
				}
			}
		}

		public void addPlayer(string playerName, int playerNum) {
			GameObject go = (GameObject)Instantiate(playerPrefab);
			if (!gameStarted)
				go.renderer.enabled = false;
			Player p = go.GetComponent<Player>();
			SpriteRenderer sp = p.GetComponent<SpriteRenderer>();
			if (players.Count==0) sp.color = Color.blue;
			else if (players.Count==1) sp.color = Color.red;
			else if (players.Count==2) sp.color = Color.green;
			else if (players.Count==3) sp.color = Color.yellow;
			players.Add(p);
			p.name = playerName;
			p.id = playerNum;
			p.isSelf = (playerName==globalVariables.GetPlayerName());
		}

		public void removePlayer(string playerName, int playerNum) {
			ArrayList players3 = new ArrayList();
			foreach (Player p in players) players3.Add(p);
			foreach (Player p in players3) {
				if (p.name == playerName) {
					players.Remove(p);
					Destroy(p.gameObject);
				}
			}
		}

		public void addPellet(int pelletId, float x, float y, float size) {
			GameObject go = (GameObject)Instantiate(pelletPrefab);
			Pellet p = go.GetComponent<Pellet>();
			SpriteRenderer sp = p.GetComponent<SpriteRenderer>();
			sp.color = Color.yellow;
			pellets.Add(p);
			p.id = pelletId;
			p.setSize(size);
			p.setPos(x,y);
		}

		public void removePellet(int pelletId) {
			ArrayList pellets3 = new ArrayList();
			foreach (Pellet p in pellets) pellets3.Add(p);
			foreach (Pellet p in pellets3) {
				if (p.id==pelletId) {
					pellets.Remove(p);
					Destroy(p.gameObject);
				}
			}
		}


		public void startGame(int player1, float x1, float y1, int d1, int player2, float x2, float y2, int d2,int[] pelletsId,float[] pelletsX, float[] pelletsY, float[] pelletsSize) {
			gameStarted = true;
			foreach (Player p in players) {
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
			for (int n=0;n<pelletsX.Length;n++) {
				addPellet(pelletsId[n],pelletsX[n],pelletsY[n],pelletsSize[n]);
			}
		}

		public void startGame(int[] playerNums, float[] playerXs, float[] playerYs, float[] playerSizes, int[] playerDirs, int[] pelletsId,float[] pelletsX, float[] pelletsY, float[] pelletsSize) {
			gameStarted = true;
			for (int n=0;n<playerNums.Length;n++) {
				foreach (Player p in players) {
					if (p.id==playerNums[n]) {
						p.transform.position = new Vector3(playerXs[n],playerYs[n],0);
						p.setSize(playerSizes[n]);
						p.setDirection(playerDirs[n]);
						p.renderer.enabled = true;
					}
				}
			}
			for (int n=0;n<pelletsX.Length;n++) {
				addPellet(pelletsId[n],pelletsX[n],pelletsY[n],pelletsSize[n]);
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