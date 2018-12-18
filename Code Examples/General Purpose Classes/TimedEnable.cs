using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEnable : MonoBehaviour {

    public Behaviour component;
    public float delay = 1f;
    private float startTime;
    public bool toggled = false;

    public void SetTimedEnable(Behaviour CB, float t) {
        component = CB;
        delay = t;
        startTime = Time.time;
        enabled = true;
    }

    /*
    Behavior:
    If sustained, call CB on update for t time.
    If toggled, callback CB on enable and disable.
    If neither, default behavior.
    */
    public void SetTimedEnable(Behaviour CB, float t, bool toggler) {
        component = CB;
        delay = t;
        startTime = Time.time;
        enabled = true;
        toggled = toggler;
    }

    private void FixedUpdate() {
        if (startTime + delay <= Time.time) {
            component.enabled = true;
        }
    }

    private void OnEnable() {
        if (toggled) {
            component.enabled = true;
        }
    }

    private void OnDisable() {
        if (toggled) {
            component.enabled = false;
        } else {
            component.enabled = true;
        }
    }
}
