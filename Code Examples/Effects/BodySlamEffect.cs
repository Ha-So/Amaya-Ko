using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BodySlamEffect : SpiritEffect {

    protected KoMovement inkiMovement;
    [Header("Child class parameters:")]
    public Transform Ko;
    protected bool hit = false;
    protected bool wasHit = false;
    public float hitDistance = .1f;
    public float hitImpulseBasePower = 20f;
    public NonRotatingFollower nonRotatingFollower;
    protected Vector3 NRFPos;
    protected Vector2 toKo;
    protected float originalDistanceToKo;
    protected bool sawKo = false;
    public bool joinKo = false;
    protected Timer resetTimer;
    private SpiritController2D controller;
    protected bool sawKoBefore = false;
    public float mercyTime = 5f;
    private int hitCount = 0;
    private Timer finaleTimer;
    private bool final = false;
    private GameObject amaya;
    public DialogueTrigger finalDT;

    protected override void Start() {
        amaya = (GameObject.FindGameObjectWithTag("OneAndOnlyAmaya"));
        finaleTimer = new Timer(3f);
        resetTimer = new Timer(mercyTime);
        base.Start();
        inkiMovement = host.GetComponent<KoMovement>();
        controller = host.GetComponent<SpiritController2D>();
        toggle = false;     // turns itself off after a time.
        triggered = true;   // one-off invoked by event.
        invoked = false;
        wasInvoked = false;
        OnTrigger = false;
        seesAmaya = false;
        sawAmaya = false;
        seesKo = false;
        actionTriggered = false;
        sustained = false;
    }

    /*
     Keep the nonrotating follower 
         */

    private void Awake() {
        actionTriggered = false;
        seesKo = false;
        sawKo = false;
        OnTrigger = false;
        sawKoBefore = false;
        toggle = false;
        triggered = true;
        hit = false;
        wasHit = false;
        //Debug.Log("How many times is this called?");
    }
    private void FixedUpdate() {
        //Debug.Log("FixedUpdating");
        toKo = new Vector2(Ko.position.x - transform.position.x, Ko.position.y - transform.position.y);

        if (final && finaleTimer.GetElapsed()) {
            gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
            final = false;
        }

        if (hit && resetTimer.GetElapsed()) {
            MercyIsForTheYang();
        }

        //Debug.Log("seesko: " + seesKo + ". SawKo: " + sawKo + ". SawKoBefore: " + sawKoBefore);
        if (!sawKo) {
            sawKo = seesKo;
        }

        if (sawKo && !sawKoBefore) {
            //Debug.Log("First sight of Ko. Snaptargeting Ko and getting Dradis.");
            inkiMovement.snapTarget = Ko.transform;
            originalDistanceToKo = (NRFPos - Ko.position).magnitude;
        }

        if (sawKo) {    //  Done having seen Ko when returned to flight path.
            Invoker("BodySlamEffect:FixedUpdate");  // invokes after saw Ko.
            CheckHit();
        }

        seesKo = false;
        sawKoBefore = sawKo;
    }

    public void MercyIsForTheYang() {
        Debug.Log("Mercy is for the Yang.");
        hitCount++;
        hit = false;
        actionTriggered = false;
        seesKo = false;
        sawKo = false;
        OnTrigger = false;
    }

    protected void Seek() {
        //Debug.Log("Seeking");
        inkiMovement.SnapToSnapPosition(inkiMovement.snapTarget.position, true);
    }

    protected void Hit() {
        //Debug.Log("Hit registered at " + Time.time);
        Rigidbody2D KoRB2D = Ko.GetComponent<Rigidbody2D>();
        KoRB2D.AddForce((hitImpulseBasePower + originalDistanceToKo) * toKo, ForceMode2D.Impulse);
        Vector2 v2 = new Vector2(-toKo.x, -toKo.y);
        rb2d.AddForceAtPosition((hitImpulseBasePower + originalDistanceToKo) * v2 * 10, rb2d.position + Vector2.down * .1f, ForceMode2D.Impulse);
        resetTimer.Start();
        //Debug.LogError("should stop now.");
        controller.Freeze(.5f);
        hit = true;
        sawKo = false;
        controller.StopSnapBraking("BodySlamEffect");
        audioSource.Play();
        Finale();
    }

    public void Finale() {
            
            inkiMovement.trailTarget = amaya.transform;
            finaleTimer.Start();
            final = true;
    }

    protected void JoinKo() {
        GameObject.Find("JoiningBodySlam").GetComponent<DialogueTrigger>().TriggerDialogue();
        animator.SetBool("Joined", true);
        Ko.gameObject.GetComponent<Animator>().SetBool("Joined", true); // terminal for Ko animation.
    }

    protected void CheckHit() {
        Vector2 offset = transform.position - Ko.position;
        if (offset.magnitude < hitDistance) {
            if (!joinKo) { // can be set by an event trigger publicly.
                inkiMovement.snapTarget = nonRotatingFollower.transform;
                Hit();
            } else {
                JoinKo();
            }
        }
    }

    override protected void Invoker(string caller) {
        //Debug.Log("Invoked by: " + caller);
        if (!hit) {
            //Debug.Log("No previous hit. Seeking.");
            Seek();
        } else {
            Disinvoker("override protected void Invoker");
        }
    }

    protected override void OnFirstInvoke() {
        base.OnFirstInvoke();
        hit = false;
    }
}
