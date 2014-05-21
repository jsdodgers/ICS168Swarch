using UnityEngine;
using System.Collections;

namespace Swarch {
	public class GameState : MonoBehaviour {

		public ArrayList players;
		public ArrayList pellets;
		public bool gameStarted;
		public GameObject playerPrefab;
		public GameObject pelletPrefab;
	
		// Use this for initialization
		void Start () {
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
					Destroy(p.gameObject);
					players.Remove(p);
				}
			}
		}

		public void startGame() {
			gameStarted = true;
			foreach (Player p in players) {
				p.renderer.enabled = true;
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}