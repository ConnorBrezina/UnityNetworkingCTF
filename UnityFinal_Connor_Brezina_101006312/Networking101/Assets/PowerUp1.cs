using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Drop()
	{
		CTFGameManager Manager = GameObject.Find ("CTFGameManager").GetComponent<CTFGameManager>();
		Destroy (GameObject.FindGameObjectWithTag ("Flag"));
		Manager.SpawnFlag();

	}
	public void OnCollisionEnter(Collision other)
	{
		if (other.collider.tag == "Player") {
			Drop ();
			Destroy (this.gameObject);
		}
	}
}
