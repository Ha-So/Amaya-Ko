using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnWalkingThruForestOff : MonoBehaviour {

    public WalkingThruForest walkingThruForest;

    private void OnEnable() {
        walkingThruForest.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
