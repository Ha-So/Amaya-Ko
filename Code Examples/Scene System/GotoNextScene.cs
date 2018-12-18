using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoNextScene : MonoBehaviour {

    public NextScene nextScene;
    private void OnEnable()
    {
        nextScene.LoadNext();
    }
}
