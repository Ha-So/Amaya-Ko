using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveLevel : MonoBehaviour {

	public void Start()
	{
		
		PlayerPrefs.SetInt ("CurrentLevel", SceneManager.GetActiveScene().buildIndex);
		PlayerPrefs.Save(); 
		
	}

}
