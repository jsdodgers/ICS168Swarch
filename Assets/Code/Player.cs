using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	Global globalVariables;
	public GameObject player;
	float maxSpeed, speedX, speedY, size;
	public KeyCode moveLeft, moveRight, moveUp, moveDown;
	private KeyCode moveLeft2, moveRight2, moveUp2, moveDown2;
	bool resetCalled;

	// Use this for initialization
	void Start()
	{
		resetCalled = false;
		gameObject.name = "Player";
		globalVariables = (Global)GameObject.FindObjectOfType(typeof(Global));
		maxSpeed = 3;
		size = 1;
		int dir = Random.Range(1,5);
		speedX = (dir%2==1?GetCurrentSpeed()*(dir-2):0);
		speedY = (dir%2==1?0:GetCurrentSpeed()*(dir-3));
		transform.position = new Vector3(Random.Range(-30, 30)/10.0f,Random.Range(-30, 30)/10.0f, 0.0f);
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

	void PlayerMove()
	{
		PlayerMoveLeft();
		PlayerMoveRight();
		PlayerMoveUp();
		PlayerMoveDown();

		transform.Translate(Time.deltaTime*speedX, Time.deltaTime*speedY, 0.0f);
	}

	void PlayerMoveLeft()
	{
		if(Input.GetKeyDown(moveLeft) || Input.GetKeyDown(moveLeft2))
		{
			speedX = -GetCurrentSpeed();
			speedY = 0;
		}
	}

	void PlayerMoveRight()
	{
		if(Input.GetKeyDown(moveRight) || Input.GetKeyDown(moveRight2))
		{
			speedX = GetCurrentSpeed();
			speedY = 0;
		}
	}

	void PlayerMoveUp()
	{
		if(Input.GetKeyDown(moveUp) || Input.GetKeyDown(moveUp2))
		{
			speedX = 0;
			speedY = GetCurrentSpeed();
		}
	}

	void PlayerMoveDown()
	{
		if(Input.GetKeyDown(moveDown) || Input.GetKeyDown(moveDown2))
		{
			speedX = 0;
			speedY = -GetCurrentSpeed();
		}
	}

	void PlayerScale()
	{
		transform.localScale = new Vector3(Mathf.Sqrt(size), Mathf.Sqrt(size), transform.localScale.z);
	}

	void resetPlayer() {
		if (!resetCalled) {
			Instantiate(player);
			Destroy(gameObject);
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
			resetPlayer();
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 20), globalVariables.GetPlayerName());
	}
}
