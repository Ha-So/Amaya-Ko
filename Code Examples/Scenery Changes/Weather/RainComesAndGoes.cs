using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RainComesAndGoes : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip clip;
    private float tickTime;
    public float weatherAdjustmentPeriod = 3f;
    private float intensity;
    public SetRainIntensity rain;
    [Range(0f, 100f)]
        public float startingIntensityPercent = 50f;
    [Range(0f, 1f)][Tooltip(".5 will hold weather relatively steady.")]
        public float improvementThreshold = .5f;
    public int weatherChangePercentage = 10;
    [Range(-1, 100)]
    public int lowStablePointPercentage = 20;
    public float improvementStability = .25f;

    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    private void OnEnable() {
        audioSource.PlayOneShot(clip, .75f);
        rain.SetIntensity(50f);
    }

    private void AdjustRain() {
        intensity = rain.GetIntensity();
        Debug.Log("Current rain intensity is " + intensity);
        float rand = Random.Range(0f, 1f);
        bool increase = false;
        bool pauseIncrease = false;

        if (rand >= .5f) {
            increase = true;
        }
        if (intensity <= lowStablePointPercentage &&
            (rand % .1 <= (improvementStability / 10))) { // hash the rand against .1, checking .0# against 20%
            pauseIncrease = true;
        }
        
        if (!increase) {
            intensity -= (weatherChangePercentage);
            if (intensity < 0f) {
                intensity = 0f;
            }
            if (intensity > 100f) {
                intensity = 100f;
            }
            Debug.Log("Decreasing rain intensity by " + weatherChangePercentage + " percent.");
        } else if (!pauseIncrease) {
            intensity += (weatherChangePercentage);
            Debug.Log("Increasing rain intensity by " + weatherChangePercentage + " percent.");
        }

        Debug.Log("Setting new intensity to " + intensity);
        rain.SetIntensity(intensity);
    }

    private void FixedUpdate() {
        // whenever time ticks past interval of weather adjustment period, adjust rain.
        if ((Time.fixedTime % weatherAdjustmentPeriod) < (tickTime % weatherAdjustmentPeriod)) {
            AdjustRain();
        }
        tickTime = Time.fixedTime;
    }
}
