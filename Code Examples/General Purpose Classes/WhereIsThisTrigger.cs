using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhereIsThisTrigger : MonoBehaviour {


    [Header("Location reference only.")]
    [Tooltip("This function doesn't do anything but provide reference to" +
        " where the programmer can find the element below." +
        " This is used when necessity dictates that an object be stored" +
        " outside the usual organization of the hierarchy," +
        " such as with parallaxing-aligned colliders.")]
    public GameObject locationInHierarchy;

}
