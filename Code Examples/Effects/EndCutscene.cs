using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutscene : MonoBehaviour {

    private Timer timer;
    public BodySlamEffect inkiBS;
    public GameObject inkigBSGO;
    public KoMovement inkiMove;
    public KoMovement KoMove;
    public Transform InkiFaceoffSpot;
    public Transform KoFaceoffSpot;
    public Rigidbody2D log;
    public Rigidbody2D umbrella;
    bool started = false;
    public float waitTime = 1f;
    
    private void Start() {
        timer = new Timer(waitTime);
    }


    /*
     enable sets inki's body slam gameobject active.
     Inki is close enough he'll see Ko.
         */
    private void OnEnable() {
        Debug.Log("EndCutscene awake");
        KoFaceoffSpot.gameObject.SetActive(true);
        KoMove.trailTarget = KoFaceoffSpot;
        inkiMove.trailTarget = InkiFaceoffSpot;
        log.bodyType = RigidbodyType2D.Static;
        umbrella.bodyType = RigidbodyType2D.Static;

    }

    private void FixedUpdate() {
        if (!started) {
            timer.Start();
            started = true;
        }
        if (timer.GetElapsed()) {
            inkigBSGO.SetActive(true); // setting this should start the fight.
            //inkiBS.SetSeesSpirit("EndCutscene: FixedUpdate");
        }
    }
}
