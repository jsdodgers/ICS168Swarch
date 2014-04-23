using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour
{
	public string globalPlayer1Name;

	// Use this for initialization
	void Awake()
	{
		globalPlayer1Name = "";
	}
	
	// Update is called once per frame
	void Update()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void SetPlayerName(string n)
	{
		globalPlayer1Name = n;
	}

	public string GetPlayerName()
	{
		return globalPlayer1Name;
	}
}
