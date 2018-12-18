using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggerMangled : MonoBehaviour {

    public void logMangled(string logMe) {
        Debug.Log("Logger invoked: " + logMe);
    }
}
