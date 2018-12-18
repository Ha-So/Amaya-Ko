using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour {

	public GameObject block;
	public Rigidbody2D KoRB2D;
	public AudioSource audioSource;
	public AudioClip impact;
    public Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

    //public void GoToKo() {
    //    rb2d.position.Set(KoRB2D.position.x, KoRB2D.position.y);
    //    rb2d.velocity = Vector3.zero;
    //}

    private void OnEnable() {
        block.GetComponent<Renderer>().enabled = true;
        block.GetComponent<Collider2D>().enabled = true;
        audioSource.PlayOneShot(impact, 0.5F);
    }

    private void OnDisable() {
        block.GetComponent<Renderer>().enabled = false;
        block.GetComponent<Collider2D>().enabled = false;
        audioSource.PlayOneShot(impact, 0.5F);
    }
}
