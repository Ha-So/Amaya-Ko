using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActivate : MonoBehaviour
{

    public GameObject component;
    public float delay = 1f;
    private float startTime;
    public bool toggled = false;

    public void SetTimedEnable(GameObject GObj, float t)
    {
        component = GObj;
        delay = t;
        startTime = Time.time;
        enabled = true;
    }

    /*
    Behavior:
    If sustained, call CB on update for t time.
    If toggled, callback CB on enable and disable.
    If neither, default behavior.
    */
    public void SetTimedEnable(GameObject GObj, float t, bool toggler)
    {
        component = GObj;
        delay = t;
        startTime = Time.time;
        enabled = true;
        toggled = toggler;
    }

    private void FixedUpdate()
    {
        if (startTime + delay <= Time.time)
        {
            component.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (toggled)
        {
            component.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (toggled)
        {
            component.SetActive(false);
        }
        else
        {
            component.SetActive(true);
        }
    }
}
