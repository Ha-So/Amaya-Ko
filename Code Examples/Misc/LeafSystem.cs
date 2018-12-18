using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafSystem : MonoBehaviour {
	public Transform playerT;
 	public Transform particleT;
	private Vector3 playerPos; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		playerPos = playerT.position;
		playerPos += Vector3.right *25.0f;
		playerPos += Vector3.up *10.0f;
		particleT.position = playerPos;
	}
}
