using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPlatform : MonoBehaviour {

    public AudioSource creakingWood;
	public float secondsToWait = 1.5f;
	public Rigidbody2D plat;
    private bool played = false;
    public bool repeating = true;
    private int count = 0;
	// Use this for initialization
	void Start () {
        plat = GetComponent<Rigidbody2D>();
		plat.isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	  void OnCollisionEnter2D(Collision2D col)
    {
        
        Debug.Log("OnCollisionEnter2D");
		StartCoroutine(Example());
		
    }

	IEnumerator Example()
    {
        count++;
        if (count == 1 || repeating) {
            yield return new WaitForSeconds(secondsToWait);
            plat.isKinematic = false;
            if (!played) {
                creakingWood.Play();
            }
            played = true;
        }
    }
}
