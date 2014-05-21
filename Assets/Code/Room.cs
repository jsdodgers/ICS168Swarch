using UnityEngine;
using System.Collections;

namespace Swarch {
	public class Room {
		
		public int numPlayers;
		public string name;
		public int id;
		
		public Room(string theName, int thePlayers, int idNum) {
			this.name = theName;
			this.numPlayers = thePlayers;
			this.id = idNum;
		}
		
	}
}
