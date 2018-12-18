using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class amayaFalls : MonoBehaviour {

    public GameObject amaya;
    public Animator animator;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        animator.SetBool("IsDrowning", true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
