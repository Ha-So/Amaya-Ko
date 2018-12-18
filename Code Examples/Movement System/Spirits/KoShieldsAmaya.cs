using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoShieldsAmaya : MonoBehaviour {

    public KoMovement Ko;
    public PlayerMovement Amaya;
    private float startTime;
    public float shieldTime = 10f;
    public DialogueTrigger dialogue;
    public ShieldEffect shield;

    private void OnEnable() {
        shield.abilityActivated = true;
        startTime = Time.fixedTime;
    }

    private void FixedUpdate() {
        float endTime = startTime + shieldTime;
        if (Time.fixedTime < (endTime)) {
            Ko.ShieldMe(true);
        } else {
            Ko.ShieldMe(false);
            dialogue.TriggerDialogue();

            enabled = false; // we're done here.
        }
    }
}
