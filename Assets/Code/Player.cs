using UnityEngine;
using System.Collections;
using System;

namespace Swarch {
	public class Player : MonoBehaviour
	{
		Global globalVariables;
		public GameObject player;
		public bool isSelf = false;
		float maxSpeed = 4, speedX, speedY, size = 1;
		public string name;
		public int id;
		public KeyCode moveLeft, moveRight, moveUp, moveDown;
		private KeyCode moveLeft2, moveRight2, moveUp2, moveDown2;
		bool resetCalled;
		public Connection connection;
		
		// Use this for initialization
		void Start()
		{
			
			Debug.Log("Start");
			connection = GameObject.Find("Connection").GetComponent<Connection>();
			resetCalled = false;
			gameObject.name = "Player";
			globalVariables = (Global)GameObject.FindObjectOfType(typeof(Global));
		//	maxSpeed = 4;
		//	size = 1;
		//	int dir = UnityEngine.Random.Range(1,5);
		//	speedX = (dir%2==1?GetCurrentSpeed()*(dir-2):0);
		//	speedY = (dir%2==1?0:GetCurrentSpeed()*(dir-3));
		//	transform.position = new Vector3(UnityEngine.Random.Range(-30.0f, 30.0f)/10.0f,UnityEngine.Random.Range(-30.0f, 30.0f)/10.0f, 0.0f);
		}
		
		// Update is called once per frame
		void Update()
		{
			PlayerMove();
			PlayerScale();
		}
		
		float GetCurrentSpeed()
		{
			return maxSpeed/Mathf.Atan(size);
		}

		public void setDirection(int dir) {
			speedX = (dir%2==1?GetCurrentSpeed()*(dir-2):0);
			speedY = (dir%2==1?0:GetCurrentSpeed()*(dir-3));
			Debug.Log("setDirection: " + dir + " X: " + speedX + " Y: " + speedY + " size: " + size);
		}
		
		void PlayerMove()
		{
			if (isSelf) {
				PlayerMoveLeft();
				PlayerMoveRight();
				PlayerMoveUp();
				PlayerMoveDown();
			}
			transform.Translate(Time.deltaTime*speedX, Time.deltaTime*speedY, 0.0f);
		}

		void postDirection(int dir) {
			Debug.Log("postDirection(" + dir + ")");
			connection.sendCommand(Command.PlayerPosition(0,transform.position.x,transform.position.y,dir));
		}
		
		void PlayerMoveLeft()
		{
			if(Input.GetKeyDown(moveLeft) || Input.GetKeyDown(moveLeft2))
			{
				speedX = -GetCurrentSpeed();
				speedY = 0;
				postDirection(1);
			}
		}
		
		void PlayerMoveRight()
		{
			if(Input.GetKeyDown(moveRight) || Input.GetKeyDown(moveRight2))
			{
				speedX = GetCurrentSpeed();
				speedY = 0;
				postDirection(3);
			}
		}
		
		void PlayerMoveUp()
		{
			if(Input.GetKeyDown(moveUp) || Input.GetKeyDown(moveUp2))
			{
				speedX = 0;
				speedY = GetCurrentSpeed();
				postDirection(4);
			}
		}
		
		void PlayerMoveDown()
		{
			if(Input.GetKeyDown(moveDown) || Input.GetKeyDown(moveDown2))
			{
				speedX = 0;
				speedY = -GetCurrentSpeed();
				postDirection(2);
			}
		}
		
		void PlayerScale()
		{
			transform.localScale = new Vector3(Mathf.Sqrt(size), Mathf.Sqrt(size), transform.localScale.z);
		}
		
		void resetPlayer() {
			if (!resetCalled) {
			//	Instantiate(player);
			//	Destroy(gameObject);
				resetCalled = true;
			}
		}
		
		void OnTriggerEnter2D(Collider2D coll)
		{
			if(coll.gameObject.name == "Pellet")
			{
				size += coll.gameObject.GetComponent<Pellet>().GetPelletSize();
			}
			
			if (coll.gameObject.name == "Player") {
				Player otherPlayer = coll.gameObject.GetComponent<Player>();
				if (size > otherPlayer.size) {
					size += otherPlayer.size;
					otherPlayer.resetPlayer();
				}
				else {
					if (size==otherPlayer.size) {
						otherPlayer.resetPlayer();
					}
					resetPlayer();
				}
			}
			
			if(coll.gameObject.name == "Wall")
			{
				/*	if (coll.transform.localScale.x == 1) {
				Debug.Log("WallX");
				Vector3 vec = transform.position;
				if (speedX>0 && vec.x>0 || speedX<0 && vec.x<0)
					vec.x = -vec.x;
				transform.position = vec;
			}
			else
			{
				Debug.Log("WallY");
				Vector3 vec = transform.position;
				
				if (speedY>0 && vec.y>0 || speedY<0 && vec.y<0)
					vec.y = -vec.y;
				transform.position = vec;
			}
			*/
				resetPlayer();
				
			}
		}

	}
}