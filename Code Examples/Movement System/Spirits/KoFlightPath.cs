using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class KoFlightPath : MonoBehaviour {

    public KoMovement Spirit;
    [Header("Events (see tooltip):")]
    [Tooltip("Do not set OnArrive to Arrive(). Arrive() should be called by the trigger collider." +
        " It invokes this, and does other things. These event callers are provided for child method utilization.")]
    public bool tooltip;
    public UnityEvent OnArrive;
    public UnityEvent EnableVCamOnStart;
    public UnityEvent PanBackToOriginal;
    public UnityEvent DisableVCamOnEnd;
    public Cinemachine.CinemachineVirtualCamera KoCam;
    public float delayAfterArrival;
    public Transform[] destinations;
    protected Transform originalTrailTarget;
    protected int count;
    public bool followWithKoCam = true;
    private bool done;
    public bool loop = false;
    private TimedCallback TCB;

    // Update is called once per frame
    protected virtual void OnEnable () {
        done = false;
		if (OnArrive == null) { OnArrive = new UnityEvent(); }
        if (followWithKoCam) {
            if (EnableVCamOnStart == null) { EnableVCamOnStart = new UnityEvent(); }
            if (DisableVCamOnEnd == null)  { DisableVCamOnEnd  = new UnityEvent(); }
            if (PanBackToOriginal == null) { PanBackToOriginal = new UnityEvent(); }
        }
        count = 0;
        // set Ko's follow to the first destination.
        // set the camera to follow Ko.
        originalTrailTarget = Spirit.trailTarget;
        SetKoFollow();
        if (followWithKoCam) {
            EnableVCamOnStart.Invoke();
        }
        destinations[count].gameObject.SetActive(true);
    }

    protected virtual void OnDisable() {

    }
    
    protected virtual void SetKoFollow() {
        destinations[count].gameObject.SetActive(true);
        Spirit.trailTarget = destinations[count];
    }

    protected virtual void SetKoReturn() {
        if (!done) {
            Spirit.trailTarget = originalTrailTarget;
            if (followWithKoCam) {
                DisableVCamOnEnd.Invoke(); }
            DoStuffOnReturn();
            done = true;
            enabled = false;
        }
    }

    public virtual void Arrive() {
        OnArrive.Invoke();
        destinations[count].gameObject.SetActive(false);
        count++;

        if (count < destinations.Length) {
            TCB = gameObject.AddComponent<TimedCallback>();
            TCB.SetTimedCallback(SetKoFollow, delayAfterArrival);
        }

        if (count == destinations.Length && !loop) {
            TCB = gameObject.AddComponent<TimedCallback>();
            TCB.SetTimedCallback(SetKoReturn, delayAfterArrival + 3.0f);
        } else if (count == destinations.Length && loop) {
            count = 0;
            TCB = gameObject.AddComponent<TimedCallback>();
            TCB.SetTimedCallback(SetKoFollow, delayAfterArrival);
        }
    }

    protected virtual void DoStuffOnReturn() {

    }
}