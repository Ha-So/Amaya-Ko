using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCallback : MonoBehaviour {

    public Callback callback;
    public float delay = 1f;
    private float startTime;
    public bool sustained = false;
    public bool toggled = false;

    // default behavior: callback once at end of time.
    public void SetTimedCallback(Callback CB, float t) {
        callback = CB;
        delay = t;
    }

    private void Start() {
        startTime = Time.time;
        enabled = true;
    }

    /*
    Behavior:
    If sustained, call CB on update for t time.
    If toggled, callback CB on enable and disable.
    If neither, default behavior.
    */
    public void SetTimedCallback(Callback CB, float t, bool toggler, bool sustain) {
        callback = CB;
        delay = t;
        startTime = Time.time;
        enabled = true;
        sustained = sustain;    // doesn't call it twice in the first round.
        if (sustained) { toggled = toggler; } // toggle doesn't matter w/ sustainment.
    }

    private void OnEnable() {
        if (!sustained && toggled) {
            callback();
        }
    }

    private void OnDisable() {
        if (!sustained) { // doesn't call it twice in the last round.
            callback();
        }
    }

    public void touch() {  }

    // started at runtime instantiation
    private void Update() {
        if (sustained) {
            callback();
        }
        if (startTime + delay <= Time.time) {
            enabled = false;
        }
    }
}
