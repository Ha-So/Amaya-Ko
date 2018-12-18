using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EOL : MonoBehaviour {

    public NextScene nextSceneScript;
    private void OnEnable()
    {
        nextSceneScript.LoadNext();
    }

}
