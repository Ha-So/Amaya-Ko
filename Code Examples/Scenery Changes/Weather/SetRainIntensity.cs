using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRainIntensity : MonoBehaviour {

    private DigitalRuby.RainMaker.RainScript2D rain;

    private void Start() {
        rain = FindObjectOfType<DigitalRuby.RainMaker.RainScript2D>();
    }

    public float GetIntensity() {
        return rain.RainIntensity * 100;
    }

    public void SetIntensity(float intensity) {
        rain.RainIntensity = intensity / 100;
    }
}
