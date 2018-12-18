using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpiritEffect : MonoBehaviour
{
    /* Members */
    public Transform host;
    public SpriteRenderer SR;
    public Rigidbody2D rb2d;
    public Animator animator;
    public AudioSource audioSource;
    public bool abilityActivated = false;
    [Header("Invocation method (tooltip):")][Tooltip("If triggered, add triggers below." +
        "If not, add button check text below. Untoggled effects are one-offs.\n" +
        "With buttons, untoggled effects are sustained as long as the button is pressed and held, and toggled effects are turned on and off whenever the button is pressed.")]
    public bool triggered = false;
    public bool toggle = false; 
    public bool sustained = false;
    [Header("Button info (tooltips):")]
    [Tooltip("AnimationText is the parameter you'll be calling in the animator.")]
    public string animationText;
    [Tooltip("ButtonText is the name of the button" +
        " in the Unity Input settings, e.g. 'Confirm'.")]
    public string buttonText = "Confirm";
    public bool invoked = false;
    protected bool wasInvoked = false;
    protected bool OnTrigger = false;
    [Header("Triggers (See tooltip):")]
    [Tooltip("The first trigger is always the one whose layer mask looks for Ko.")]
    protected int count = 3;
    public bool seesAmaya = false;
    public bool sawAmaya = false;
    public bool seesKo = false;
    public bool actionTriggered = false;

    // undefined callbacks

    virtual protected void Start () {
        triggered = false;
        SR = host.gameObject.GetComponent<SpriteRenderer>();
        rb2d = host.gameObject.GetComponent<Rigidbody2D>();
        animator = host.gameObject.GetComponent<Animator>();
    }

    public void SetSeesPlayer() { // called by triggeraction on enter/exit.
        if (!actionTriggered) {
            seesAmaya = true;
            actionTriggered = true;
        }
    }

    public void SetSeesSpirit(string caller) { // called by triggeraction on enter/exit.
        if (!actionTriggered) {
            seesKo = true;
            actionTriggered = true;
            //Debug.Log("SetSeesSpirit set by: " + caller + ". Action Triggered.");
        }
    }

    private void ButtonInvoker() {
        if (buttonText != "" && buttonText != null) { 
            if (sustained) {
                if (Input.GetButton(buttonText)) {

                    Invoker("ButtonInvoker: OnTrigger:" + OnTrigger +
                    ", Toggle: " + toggle + ", sustained: " + sustained +
                    ", triggered: " + triggered);
                    invoked = true;

                } else if (Input.GetButtonUp(buttonText)) {
                    invoked = false;
                    Disinvoker("ButtonInvoker: OnTrigger:" + OnTrigger +
                    ", Toggle: " + toggle + ", sustained: " + sustained +
                    ", triggered: " + triggered); // not sure if this is needed.
                } // end keycheck
            } else if (toggle) {
                if (!invoked && Input.GetButtonDown(buttonText)) {
                    Debug.Log("Invoking button.");
                    Invoker("ButtonInvoker: OnTrigger:" + OnTrigger +
                    ", Toggle: " + toggle + ", sustained: " + sustained +
                    ", triggered: " + triggered);
                    invoked = true;
                } else if (invoked && Input.GetButtonDown(buttonText)) {
                    Debug.Log("Disinvoking button.");
                    Disinvoker("ButtonInvoker: OnTrigger:" + OnTrigger +
                    ", Toggle: " + toggle + ", sustained: " + sustained +
                    ", triggered: " + triggered);
                    invoked = false;
                } // end keycheck
            } else {
                if (Input.GetButtonDown(buttonText)) {
                    Invoker("ButtonInvoker: OnTrigger:" + OnTrigger +
                    ", Toggle: " + toggle + ", sustained: " + sustained +
                    ", triggered: " + triggered);
                    invoked = true;
                } else if (Input.GetButtonUp(buttonText)) {
                    Disinvoker("ButtonInvoker: OnTrigger:" + OnTrigger +
                    ", Toggle: " + toggle + ", sustained: " + sustained +
                    ", triggered: " + triggered);
                    invoked = false;
                } // end keycheck
            }
        }
    }

    /* 
    * Trigger effects are not sustained, but multiple enter / exit
    * events may happen at once, based on dynamic movement.
    * OnTrigger gets only the first until an exit is found.
    * So this may oscillate a bit.
    */
    // triggers may be intermittently sustained. So make them terminal or 
    //   sustain independently in child class updates, such as with 'seesAmaya' set true.

    /*
        Trigger invoker: doesn't use a button.
        Trigger toggle invoker: trigger once to turn it on, again to turn it off,
            usually by and entry and exit.
        Trigger non-toggle invoker: One call: Fire and forget.

        You want it sustained, do it yourself.
        Sustained trigger conditions are unreliable.
    */

    private void TriggeredInvoker() {   // one time, fire and forget. As intended.
        if (OnTrigger) {
            OnTrigger = false;
            if (!toggle) {      // trigger to sustain at least .5s. Have Invoker() set .invoked to true to immediately disinvoke.
                Invoker("TriggeredInvoker: OnTrigger:" + OnTrigger +
                ", Toggle: " + toggle + ", sustained: " + sustained +
                ", triggered: " + triggered);
            }  // end Toggle vs. One-off check
        } // end Trigger check
    }

    private void TriggeredToggleInvoker() { // 2 times, fire on and fire off.
        if (OnTrigger) {
            Invoker("TriggeredInvoker: OnTrigger:" + OnTrigger +
            ", Toggle: " + toggle + ", sustained: " + sustained +
            ", triggered: " + triggered);
            invoked = true;
        } else { 
            Disinvoker("TriggeredInvoker: OnTrigger:" + OnTrigger +
                ", Toggle: " + toggle + ", sustained: " + sustained +
                ", triggered: " + triggered);
            invoked = false;
        }
    }

	virtual protected void Update () {
        wasInvoked = invoked;
        if (!triggered) {
            ButtonInvoker();
        } else if (toggle) {
            TriggeredToggleInvoker();
        } else {
            TriggeredInvoker();
        }

        // reset only twice per second.

        animator.SetBool(animationText, invoked);
        if (!wasInvoked && invoked) { OnFirstInvoke(); }
        if (wasInvoked && !invoked) { OnFirstDisinvoke(); }
    }
    virtual protected void OnFirstInvoke() {
        audioSource.Play();
    }

    virtual protected void OnFirstDisinvoke() { }

    /* DON'T USE OnEnable!!! */
    virtual protected void Invoker(string caller) { }
    virtual protected void Disinvoker(string caller) {
        actionTriggered = false;
        //Debug.Log("Disinvoker called.");
    }
    private void OnDisable() {
        seesAmaya = false; // why is this here? Some previous hacking? Pretty sure I put this here for a reason at some point.
    }
}
