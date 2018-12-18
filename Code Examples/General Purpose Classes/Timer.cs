using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {

    private float duration;
    private float end;
    public Timer(float duration) {
        this.duration = duration;
    }

    public void SetDuration(float duration) {
        this.duration = duration;
    }

    public void Start() {
        end = Time.time + duration;
        //Debug.Log("Timer started at " + Time.time);
    }

    public bool GetElapsed() { // call in (fixed)update.
        if (end <= Time.time) {
            //Debug.Log("Timer Elapsed. returning true.");
        }
        return (end <= Time.time);
    }

}
