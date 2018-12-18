using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlockEffect : SpiritEffect {

    private KoMovement Ko;
    [Header("Child class parameters:")]
    public IceBlock iceBlock;

    protected override void Start() {
        toggle = true;
        triggered = false;
        Ko = host.GetComponent<KoMovement>();
    }

    override protected void Invoker(string bla) {
        Debug.Log("Ice block invoked");
        SR.enabled = false;
        iceBlock.gameObject.SetActive(true);
        iceBlock.enabled = true;
        iceBlock.transform.position = Ko.transform.position;
    }

    override protected void Disinvoker(string bla) {
        Ko.transform.position = iceBlock.transform.position;
        rb2d.velocity = Vector3.zero;
        SR.enabled = true;
        iceBlock.enabled = false;
        iceBlock.gameObject.SetActive(false);
    }
}
