using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPlunge : MonoBehaviour {

    public PlayerMovement amaya;
    public NextScene loadNext;
    public ResetLevel resetLevel;
    private Timer timer;
    public Rigidbody2D log;
	// Use this for initialization
	void Start () {
        resetLevel.gameObject.SetActive(false);
        timer = new Timer(2);
	}

    private void OnEnable() {
        timer.Start();
    }

    private void FixedUpdate() {
        if (timer.GetElapsed()) {
            loadNext.LoadNext();
        }
    }
}

