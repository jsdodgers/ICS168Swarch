using UnityEngine;
using System.Collections;

namespace Swarch {
	public class Rooms {
		ArrayList rooms;
		public Rooms() {
			rooms = new ArrayList();	
		}
		
		public void addRoom(string name, int numPlayers,int idNum) {
			bool has = false;
			foreach (Room r in rooms) {
				if (r.id==idNum) {
					r.name = name;
					r.numPlayers = numPlayers;
					has = true;
				}
			}
			if (!has) {
				rooms.Add(new Room(name,numPlayers,idNum));
			}
		}
		
		public void printRoomsToConsole() {
			string str = "";
			foreach (Room r in rooms) {
				str+= r.id;
				str+= " " + r.name;
				str+= " " + r.numPlayers + "\n";
			}
			
		}
		public int count() {
			return rooms.Count;
		}
		
		public Room get(int n) {
			return (Room)rooms[n];
		}

		public Room getWithId(int id) {
			foreach (Room r in rooms) {
				if (r.id==id) {
					return r;
				}
			}
			return null;
		}
	}
}
