using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightningFlash : MonoBehaviour {

    public Canvas canvas;
    public Vector2[] startAndEndTimes;
    private float startTime;
    private float endTime;
    private int count = 0;
    private bool timeSetForThisRound = false;
    private float beginTime;

    private void Whiteout(bool now) {
        canvas.enabled = now;
    }

    private void SetTimes(int index) {
        if (!timeSetForThisRound) {
            startTime = beginTime + startAndEndTimes[index].x;
            endTime = beginTime + startAndEndTimes[index].y;
            timeSetForThisRound = true; // only sets once.
        }
    }

    private void Update() {
        if (count < startAndEndTimes.Length && !timeSetForThisRound) {
            SetTimes(count); // only called once per count round.
        }
        if (endTime < Time.time) {
            Whiteout(false);
            timeSetForThisRound = false;
            count++;
        } else if (startTime < Time.time) {
            Whiteout(true);
        }

        if (count >= startAndEndTimes.Length) {
            count = 0;
            timeSetForThisRound = false;
            this.enabled = false;
        }
    }

    private void OnEnable() {
        beginTime = Time.time;
    }
}
