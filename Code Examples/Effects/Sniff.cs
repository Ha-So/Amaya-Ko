using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniff : MonoBehaviour {

    public Animator amaya;

    private void OnEnable() {
        amaya.SetBool("DriedOff", true);
        gameObject.SetActive(false);
    }
}
