using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFlashEffect : SpiritEffect {

    // enable lightning flash
    // still to code: stop moving while she's in sight.
    [Header("Child class parameters:")]
    public LightningFlash flash;
    public GameObject littleDarkCloud;
    protected KoMovement inkiMovement;
    protected bool invokedOnce = false;
    public Transform amaya;
    

	// Use this for initialization
	override protected void Start () {
        base.Start();
        sustained = true;
        triggered = true;
        toggle = false;
        inkiMovement = host.GetComponent<KoMovement>();
	}

    // Update is called once per frame
    override protected void Update () {
        base.Update();
        if (seesAmaya) {
            Debug.Log("Amaya seen.");
            OnTrigger = true;
        } else {
            OnTrigger = false;
            Disinvoker("LightningFlashEffect:Update");
        }
	}

    public void ResetTrigger() {
        actionTriggered = false;
    }


    override protected void OnFirstInvoke() {
        if (!invokedOnce) {
            audioSource.Play();
            //Debug.Log("First invoke called.");
            flash.enabled = true;
            invokedOnce = true;
            inkiMovement.snapTarget = littleDarkCloud.transform;
        }
    }

    protected override void Invoker(string bla) { // Should self-sustain after first invocation.
        Debug.Log("Invoking Lightning effect.");
        base.Invoker("");
        OnFirstInvoke();
        inkiMovement.SnapToSnapPosition(inkiMovement.snapTarget.position, true); // assumes fired sustained?
    }

    protected override void Disinvoker(string bla) { // assumes only fired once when amaya not seen.
        inkiMovement.SnapToSnapPosition(Vector2.zero, false);
        base.Disinvoker("base.Disinvoker");
        //littleDarkCloud.SetActive(false);
        invokedOnce = false;
    }
}
