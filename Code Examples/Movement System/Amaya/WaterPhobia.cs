using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPhobia : MonoBehaviour {

    public int raindropHits;
    public int maxRaindrops;
    private float automaticDryoffDelay;
    [Tooltip("Number of seconds to dry off after a 5 second delay")][Range(0, 30)]
        public int secondsToDryOff;
    private float lastTimeHitByRaindrop;
    private bool wet;
    private float tick;

    private void FixedUpdate() {
        tick = Time.fixedTime;
        if (tick > 1f) {
            CheckAutoDryoff();
            tick -= 1f;
        }

        if ((!wet) && (raindropHits >= maxRaindrops)) {
            SendMessageUpwards("Object_GetWet");
            wet = true;
        }
    }

    private void CheckAutoDryoff() {
        if (Time.fixedTime > lastTimeHitByRaindrop + automaticDryoffDelay) {
            raindropHits = ((maxRaindrops / secondsToDryOff > raindropHits) ? 0 : 
                (raindropHits - (maxRaindrops / secondsToDryOff)));
        }

        if (raindropHits == 0) {
            SendMessageUpwards("Object_DryOff");
            wet = false;
        }
    }

    private void OnEnable() {
        wet = false;
        automaticDryoffDelay = 5f;
    }

    public void TheWettening()
    {
        raindropHits = maxRaindrops;
        wet = true;
        lastTimeHitByRaindrop = Time.fixedTime;
        tick = 0f;
        SendMessageUpwards("Object_GetWet");
    }
    public void DryByExternalEffect() {
        raindropHits = 0;
        wet = false;
    }

    public void HitByRaindrop() {
        if (raindropHits <= maxRaindrops) {
            raindropHits++;
        }
//        Debug.Log("Hit By Raindrop at " + Time.fixedTime);
        lastTimeHitByRaindrop = Time.fixedTime; // reset timer;
        tick = 0f;
    }
}
