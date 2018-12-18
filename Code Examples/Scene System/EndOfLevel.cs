using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevel : MonoBehaviour {

    void OnEnable()
    {
        SendMessage("LoadNext");
    }
}
