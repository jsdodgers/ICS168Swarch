using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	Global globalVariables;
	public GameObject player;
	float maxSpeed, speedX, speedY, size;
	public KeyCode moveLeft, moveRight, moveUp, moveDown;

	// Use this for initialization
	void Start()
	{
		gameObject.name = "Player";
		globalVariables = (Global)GameObject.FindObjectOfType(typeof(Global));
		maxSpeed = 3;
		size = 1;
		int dir = Random.Range(1,5);
		speedX = (dir%2==1?GetCurrentSpeed()*(dir-2):0);
		speedY = (dir%2==1?0:GetCurrentSpeed()*(dir-3));
		transform.position = new Vector3(Random.Range(-9, 9),Random.Range(-3, 3), 0.0f);
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
		if(Input.GetKeyDown(moveLeft))
		{
			speedX = -GetCurrentSpeed();
			speedY = 0;
		}
	}

	void PlayerMoveRight()
	{
		if(Input.GetKeyDown(moveRight))
		{
			speedX = GetCurrentSpeed();
			speedY = 0;
		}
	}

	void PlayerMoveUp()
	{
		if(Input.GetKeyDown(moveUp))
		{
			speedX = 0;
			speedY = GetCurrentSpeed();
		}
	}

	void PlayerMoveDown()
	{
		if(Input.GetKeyDown(moveDown))
		{
			speedX = 0;
			speedY = -GetCurrentSpeed();
		}
	}

	void PlayerScale()
	{
		transform.localScale = new Vector3(Mathf.Sqrt(size), Mathf.Sqrt(size), transform.localScale.z);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if(coll.gameObject.name == "Pellet")
		{
			size += coll.gameObject.GetComponent<Pellet>().GetPelletSize();
		}

		if(coll.gameObject.name == "Wall")
		{
			Instantiate(player);
			Destroy(gameObject);
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 20), globalVariables.GetPlayerName());
	}
}
