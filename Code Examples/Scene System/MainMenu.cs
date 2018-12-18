using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour {

    public Canvas loadMenu;
    public Canvas noSaveMenu;
    public Canvas quitMenu;
    public Button exitText;
    public Button startText;
    public Button loadText;
    public Button creditText;
    // Use this for initialization
    void Start()
    {
        quitMenu = quitMenu.GetComponent<Canvas>();
        noSaveMenu = noSaveMenu.GetComponent<Canvas>();
        loadMenu = loadMenu.GetComponent<Canvas>(); 
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
        loadText = loadText.GetComponent<Button>();
        creditText = creditText.GetComponent<Button>();
        quitMenu.enabled = false;
        noSaveMenu.enabled = false; 
        loadMenu.enabled = false; 

    }

    public void BookmarksPress()
    {
        if(PlayerPrefs.HasKey("CurrentLevel") == false)
        {
            noSaveMenu.enabled = true;
            startText.enabled = false;
            exitText.enabled = false;
            loadText.enabled = false;
            creditText.enabled = false;
        }
        else
        {
            loadMenu.enabled = true; 
            startText.enabled = false;
            exitText.enabled = false;
            loadText.enabled = false;
            creditText.enabled = false;
        }
    }
    public void ExitPress()
    {
        
        quitMenu.enabled = true;
        startText.enabled = false;
        exitText.enabled = false;
        loadText.enabled = false;
        creditText.enabled = false;
    }

    public void NoPress()
    {
        loadMenu.enabled = false;
        noSaveMenu.enabled = false;
        quitMenu.enabled = false;
        startText.enabled = true;
        exitText.enabled = true;
        loadText.enabled = true;
        creditText.enabled = true;
    }
   

    public void ExitGame()
    {
        Application.Quit();
    }

    public void NewGameLoad()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void deletePrefs()
    {
        PlayerPrefs.DeleteAll();
        NoPress(); 

    }

    public void loadPrefs()
    {
        SceneManager.LoadScene(PlayerPrefs.GetInt("CurrentLevel"));
    }
    public void openCredits()
	{
		SceneManager.LoadScene("Credits", LoadSceneMode.Single);
	}
}
