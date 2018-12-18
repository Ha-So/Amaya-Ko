using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 using System;

public class FadeText : MonoBehaviour {
    
	private TMP_Text m_TextComponent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
	void Awake () {
		 m_TextComponent = GetComponent<TMP_Text>();
		//StartCoroutine(FadeTextToZeroAlpha(1000000000f, m_TextComponent));
		
		 TMP_Text i = m_TextComponent;
		i.color = new Color32(0,120, 255, 255);
		int x = 10;
        while (i.color.a > 0.0f)
        {
			Debug.Log("Fade1");
            i.color = new Color32(0, 255, 255, Convert.ToByte(x));
			 StartCoroutine(MyMethod());
			 Debug.Log("Fade2");
		}
        //m_TextComponent.text = "A line of text.";
        //m_TextComponent.color = Color.yellow;
        //m_TextComponent.outlineWidth = 0.15f;
       // m_TextComponent.outlineColor = Color.red;
	   /* for(int i = 255; i > 0; i--)
	   {
		   m_TextComponent.color = new Color32(0, 255, 255, Convert.ToByte(i));
	   }*/
    }    

	 public IEnumerator FadeTextToZeroAlpha(float t, TMP_Text i)
    {
		//int currentColor = 255;
        i.color = new Color32(0,120, 255, 255);
        while (i.color.a > 0.0f)
        {
            i.color = new Color32(0, 255, 255, Convert.ToByte(i.color.a - (Time.deltaTime / t)));
            yield return null;
        }
    }

	IEnumerator MyMethod() {
		Debug.Log("Before Waiting 2 seconds");
		yield return new WaitForSeconds(1);
		Debug.Log("After Waiting 2 Seconds");
 	}
	

}
