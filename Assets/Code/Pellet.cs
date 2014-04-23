using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour
{
	public GameObject pellet;
	float pelletSize;

	// Use this for initialization
	void Start()
	{
		pelletSize = Random.Range(1, 4)*0.1f;
		gameObject.name = "Pellet";
		transform.position = new Vector3(Random.Range(-67, 67)/10,Random.Range(-30, 30)/10, 0.0f);
		transform.localScale = new Vector3(pelletSize + 0.1f, pelletSize + 0.1f, 0.0f);
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if(coll.gameObject.name == "Player")
		{
			Instantiate(pellet);
			Destroy(gameObject);
		}
	}

	public float GetPelletSize()
	{
		return pelletSize;
	}
}
