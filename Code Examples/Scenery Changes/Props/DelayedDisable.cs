using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDisable : MonoBehaviour {

    public float delay;
    private float timerStart;
    private bool started = false;
    public NextScene nextLoader;

    public void DisableWithDelay() {
        timerStart = Time.fixedTime;
        started = true;
    }

    private void Update()
    {
        if (started && Time.fixedTime > timerStart + delay) {
            started = false;
            nextLoader.LoadNext();
        }
    }
}
