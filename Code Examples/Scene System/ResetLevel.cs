using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ResetLevel : MonoBehaviour {

	public AudioClip splash;
    public AudioSource audioSource;
    private bool resetting = false;
    private float endTime;


	
    public void ResetNow() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (resetting && (endTime < Time.fixedTime)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}

    private void DelayedReset() {
        endTime = Time.fixedTime + splash.length + .1f;
        resetting = true;
    }

    private void OnEnable() {
        audioSource.PlayOneShot(splash, 1f);
        DelayedReset();
    }
}
