using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmbrellaSticks : MonoBehaviour {

    public Animator animator;
    public Transform umbrella;
    public Transform InkiFollower;

    private Vector3 pos;
    private Vector3 offset;

    private void OnEnable() {
        pos = umbrella.position;
        offset = InkiFollower.position - pos;
    }

    private void Update() {
        pos = InkiFollower.position + offset;
        umbrella.position = new Vector2(pos.x, pos.y);
    }
}
