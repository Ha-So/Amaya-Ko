using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpeed : MonoBehaviour {
	public float speedC;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.left * speedC);
	}
}
