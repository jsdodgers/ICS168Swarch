﻿using UnityEngine;
using System.Collections;

namespace Swarch {
	public class Pellet : MonoBehaviour
	{
		public GameObject pellet;
		public float pelletSize;
		public int id;
		
		// Use this for initialization
		void Start()
		{
			//	pelletSize = UnityEngine.Random.Range(1, 4)*0.1f;
			gameObject.name = "Pellet";
			//	transform.position = new Vector3(UnityEngine.Random.Range(-67.0f, 67.0f)/10.0f,UnityEngine.Random.Range(-30.0f, 30.0f)/10.0f, 0.0f);
			//	transform.localScale = new Vector3(pelletSize + 0.1f, pelletSize + 0.1f, 0.0f);
		}
		
		// Update is called once per frame
		void Update()
		{
			
		}
		
		public void setSize(float size) {
			pelletSize = size;
			transform.localScale = new Vector3(pelletSize + 0.1f, pelletSize + 0.1f, 0.0f);
		}
		
		public void setPos(float x, float y) {
			transform.position = new Vector3(x,y,0);
		}
		
		void OnTriggerEnter2D(Collider2D coll)
		{
			if(coll.gameObject.name == "Player")
			{
				//		Instantiate(pellet);
				//		Destroy(gameObject);
			}
		}
		
		public float GetPelletSize()
		{
			return pelletSize;
		}
	}
}