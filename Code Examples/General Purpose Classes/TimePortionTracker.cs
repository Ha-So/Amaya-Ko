using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePortionTracker {

    private float startTime;
    private float duration;

    public TimePortionTracker(float duration) {
        this.duration = duration;
    }

    public void SetDuration(float duration) {
        this.duration = duration;
    }

    public void Start() {
        startTime = Time.time;
    }

    public float GetPortion() {
        if (duration > 0f) {
            return (Time.time - startTime) / duration;
        } else {
            return -42;
        }
    }

}
