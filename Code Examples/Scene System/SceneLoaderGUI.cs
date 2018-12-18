using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneLoaderGUI : MonoBehaviour {

    public Texture groupTexture;

    // turn off during gameplay.
    private void Awake() {
        gameObject.SetActive(false);
    }

    private void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 200, 60));
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        if (GUILayout.Button(groupTexture, "Scene 1A")) {
            SceneManager.LoadScene("Level01A");
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
