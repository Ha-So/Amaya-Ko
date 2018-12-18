using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : SpiritEffect {

    [Header("Child class parameters:")]
    public KoMovement koMove;
    //private bool shielding = false;

    protected override void Start() {
        sustained = true;
    }

    override protected void Invoker(string bla) {
        koMove.ShieldMe(true);
        //shielding = true;
    }

    protected override void Disinvoker(string bla) {
        koMove.ShieldMe(false);
        //shielding = false;
    }

}

// remember to make an auto-shield effect for the final cutscene.
// Should be pretty similar.