using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FadeTextPro : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(FadeTextToZeroAlpha(6f, GetComponent<TMP_Text>()));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

		public IEnumerator FadeTextToZeroAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
