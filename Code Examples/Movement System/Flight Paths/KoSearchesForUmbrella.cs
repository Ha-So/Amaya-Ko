using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KoSearchesForUmbrella : KoFlightPath {
    protected override void DoStuffOnReturn() {
        // assumes dialogue exit state does not restore Amaya's movement if KoCam is activated.
        if (followWithKoCam) {
            FindObjectOfType<PlayerMovement>().DialogueOff();
        }
    }
}