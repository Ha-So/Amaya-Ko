using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaskedCollider2D : MonoBehaviour {

    public Collider2D collider2d;
    public LayerMask mask;
    public bool invoking = false;
    private List<GameObject> GOs;

    MaskedCollider2D(Collider2D c2d, LayerMask m) {
        collider2d = c2d;
        mask = m;
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if ((mask.value & 1 << collision.gameObject.layer) == 
                1 << collision.gameObject.layer) {
            invoking = true;
            if (!GOs.Contains(collision.gameObject))
            {
                GOs.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (GOs.Contains(collision.gameObject)) {
            GOs.Remove(collision.gameObject);
        }
        if (GOs.Count == 0) {
            invoking = false;
        }
    }
}
