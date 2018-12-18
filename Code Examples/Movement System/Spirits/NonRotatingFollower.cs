using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonRotatingFollower : MonoBehaviour {

    public Transform Ko;
    private Transform frameTarget;
    public bool stopped = false;
    public bool wasStopped = false;
    private Timer timer;
	// Use this for initialization
	void Start () {
        frameTarget = gameObject.GetComponent<Transform>();
        timer = new Timer(.25f);
	}

    public void HoldStill() { // holdstill while sustained.
        Debug.Log("HoldStill called.");
        if (timer.GetElapsed()) {
            Debug.Log("getElapsed returned true.");
            stopped = true;
        } else {
            Debug.Log("GetElapsed returned false");
        }
    }

    public void HoldStill(bool holdStill) { // holdstill while toggled.
        Debug.Log("Boolean HoldStill called with " + holdStill);
        if (timer.GetElapsed()) {
            Debug.Log("getElapsed returned true.");
            stopped = holdStill;
        } else {
            Debug.Log("GetElapsed returned false");
        }
    }

	// Update is called once per frame
	void Update () {
        frameTarget.Rotate(0, 0, 0, Space.World);
        if (!stopped) {
            frameTarget.position = new Vector3(
                Ko.position.x, Ko.position.y, Ko.position.z);
        } else {
            transform.position = new Vector3(
                transform.position.x, transform.position.y, transform.position.z);
        }
        if (!wasStopped && stopped) {
            timer.Start();
        }
        //GameObject.Find("CM KoCam").GetComponent<Cinemachine.CinemachineVirtualCameraBase>()
        wasStopped = stopped;
        stopped = false;
    }
}
