using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Use this to activate another gameobject, when that object ought
    to stay where it is, instead of being placed in the scriptEnablerHolder
    */
public class ActivateOtherGO : MonoBehaviour {

    public GameObject objectToEnable;
    public bool repeatable = false;
    public bool toggle;
    private void OnEnable() {
        bool active = objectToEnable.activeInHierarchy;
        if (toggle && active) {
            objectToEnable.SetActive(false);
        } else {
            objectToEnable.SetActive(true);
        }

        if (repeatable) {
            gameObject.SetActive(false); // repeatable.
        }
    }
}
