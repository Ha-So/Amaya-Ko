using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritAndUmbrella2D : SpiritController2D {

    public Transform UmbrellaTransform;
    protected override void Flip() {
        base.Flip();
        Vector3 theScale = UmbrellaTransform.localScale;
        theScale.x *= -1;
        UmbrellaTransform.localScale = theScale;
    }

}
