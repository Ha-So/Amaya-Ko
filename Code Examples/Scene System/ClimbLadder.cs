using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbLadder : MonoBehaviour {

    public NextScene nextSceneScript;
    // code that makes up change level instead of jump.
    // Use this for initialization
    private void FixedUpdate() {
        if (Input.GetButtonDown("Jump")) {
            nextSceneScript.LoadNext();
        }
    }
}
